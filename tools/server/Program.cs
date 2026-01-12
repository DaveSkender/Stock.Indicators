using System.Globalization;
using System.Net.ServerSentEvents;
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
    async (HttpContext context, int interval = 100, int? batchSize = null) => {
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        int delivered = 0;
        double seed = 1000.0;

        Console.WriteLine(
            $"[Random] Starting stream - interval: {interval}ms, batchSize: {batchSize?.ToString(CultureInfo.InvariantCulture) ?? "unlimited"}");

        try
        {
            while (!context.RequestAborted.IsCancellationRequested)
            {
                // Generate a random quote
                DateTime timestamp = DateTime.UtcNow.AddMinutes(delivered - 1000);
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
    async (HttpContext context, int interval = 100, int? batchSize = null) => {
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        IReadOnlyList<Quote> longestQuotes = DataLoader.GetLongest();
        int totalQuotes = batchSize ?? longestQuotes.Count;
        int delivered = 0;

        Console.WriteLine(
            $"[Longest] Starting stream - interval: {interval}ms, total: {totalQuotes} quotes");

        try
        {
            for (int i = 0; i < totalQuotes && i < longestQuotes.Count; i++)
            {
                Quote quote = longestQuotes[i];

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
Console.WriteLine("  - /quotes/random?interval=100&batchSize=1000");
Console.WriteLine("  - /quotes/longest?interval=100&batchSize=1000");

await app.RunAsync().ConfigureAwait(false);

