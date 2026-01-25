using System.Globalization;
using System.Text.Json;
using Skender.Stock.Indicators;
using Test.SseServer;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder.Build();

// Configure JSON serialization options
JsonSerializerOptions jsonOptions = new() {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// SSE endpoint: Random quotes
app.MapGet(
    "/quotes/random",
    async (HttpContext context, int interval = 100, int? batchSize = null, string quoteInterval = "1m") => {
        // Validate interval parameter
        if (interval <= 0)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Interval must be greater than 0").ConfigureAwait(false);
            return;
        }

        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        int delivered = 0;
        double seed = 1000.0;
        TimeSpan timestampIncrement = ParseInterval(quoteInterval);

        Console.WriteLine(
            $"[Random] Starting stream - delivery: {interval}ms, quoteInterval: {quoteInterval}, batchSize: {batchSize?.ToString(CultureInfo.InvariantCulture) ?? "unlimited"}");

        try
        {
            while (!context.RequestAborted.IsCancellationRequested)
            {
                // Generate a random quote with time-warped timestamp
                DateTime timestamp = DateTime.UtcNow.AddMinutes(-1000) + (timestampIncrement * delivered);
                Quote quote = DataLoader.GenerateRandomQuote(timestamp, seed);
                seed = (double)quote.Close;

                // Serialize quote as JSON
                string json = JsonSerializer.Serialize(quote, jsonOptions);

                // Write SSE event manually
                string sseData = $"event: quote\ndata: {json}\n\n";
                await context.Response
                    .WriteAsync(sseData, context.RequestAborted)
                    .ConfigureAwait(false);
                await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);

                delivered++;

                if (delivered % 100 == 0)
                {
                    Console.WriteLine($"[Random] Delivered {delivered} quotes");
                }

                // Check if we've reached the batch size limit
                if (batchSize.HasValue && delivered >= batchSize.Value)
                {
                    Console.WriteLine($"[Random] Batch complete - delivered {delivered} quotes");
                    break;
                }

                // Wait for the specified interval
                await Task.Delay(interval, context.RequestAborted).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"[Random] Client disconnected - delivered {delivered} quotes");
        }
        catch (IOException)
        {
            Console.WriteLine($"[Random] Connection closed - delivered {delivered} quotes");
        }
    });

// SSE endpoint: Longest quotes (deterministic)
app.MapGet(
    "/quotes/longest",
    async (HttpContext context, int interval = 100, int? batchSize = null, string quoteInterval = "1m") => {
        // Validate interval parameter
        if (interval <= 0)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Interval must be greater than 0").ConfigureAwait(false);
            return;
        }

        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        IReadOnlyList<Quote> longestQuotes = DataLoader.GetLongest();
        int totalQuotes = batchSize ?? longestQuotes.Count;
        int delivered = 0;
        TimeSpan timestampIncrement = ParseInterval(quoteInterval);
        DateTime baseTimestamp = longestQuotes[0].Timestamp;

        Console.WriteLine(
            $"[Longest] Starting stream - delivery: {interval}ms, quoteInterval: {quoteInterval}, total: {totalQuotes} quotes");

        try
        {
            for (int i = 0; i < totalQuotes && i < longestQuotes.Count; i++)
            {
                Quote originalQuote = longestQuotes[i];

                // Create quote with time-warped timestamp
                Quote quote = new(
                    Timestamp: baseTimestamp + (timestampIncrement * i),
                    Open: originalQuote.Open,
                    High: originalQuote.High,
                    Low: originalQuote.Low,
                    Close: originalQuote.Close,
                    Volume: originalQuote.Volume);

                // Serialize quote as JSON
                string json = JsonSerializer.Serialize(quote, jsonOptions);

                // Write SSE event manually
                string sseData = $"event: quote\ndata: {json}\n\n";
                await context.Response
                    .WriteAsync(sseData, context.RequestAborted)
                    .ConfigureAwait(false);
                await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);

                delivered++;

                if (delivered % 100 == 0)
                {
                    Console.WriteLine($"[Longest] Delivered {delivered}/{totalQuotes} quotes");
                }

                // Wait for the specified interval
                await Task.Delay(interval, context.RequestAborted).ConfigureAwait(false);
            }

            Console.WriteLine($"[Longest] Stream complete - delivered {delivered} quotes");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"[Longest] Client disconnected - delivered {delivered}/{totalQuotes} quotes");
        }
        catch (IOException)
        {
            Console.WriteLine($"[Longest] Connection closed - delivered {delivered}/{totalQuotes} quotes");
        }
    });

Console.WriteLine("SSE Server starting...");
Console.WriteLine("Endpoints:");
Console.WriteLine("  - /quotes/random?interval=100&batchSize=1000&quoteInterval=1h");
Console.WriteLine("  - /quotes/longest?interval=100&batchSize=1000&quoteInterval=5m");

await app.RunAsync().ConfigureAwait(false);

// Parse interval string (e.g., "1h", "5m", "30s") to TimeSpan
static TimeSpan ParseInterval(string interval)
{
    if (string.IsNullOrWhiteSpace(interval))
    {
        return TimeSpan.FromMinutes(1);
    }

#pragma warning disable CA1308 // ToLowerInvariant is intentional for case-insensitive parsing
    string trimmed = interval.Trim().ToLowerInvariant();
#pragma warning restore CA1308

    // Extract numeric part and unit
    int i = 0;
    while (i < trimmed.Length && (char.IsDigit(trimmed[i]) || trimmed[i] == '.'))
    {
        i++;
    }

    if (i == 0 || !double.TryParse(trimmed[..i], out double value))
    {
        return TimeSpan.FromMinutes(1);
    }

    string unit = trimmed[i..].Trim();

    return unit switch {
        "s" or "sec" or "second" or "seconds" => TimeSpan.FromSeconds(value),
        "m" or "min" or "minute" or "minutes" => TimeSpan.FromMinutes(value),
        "h" or "hr" or "hour" or "hours" => TimeSpan.FromHours(value),
        "d" or "day" or "days" => TimeSpan.FromDays(value),
        _ => TimeSpan.FromMinutes(1)
    };
}
