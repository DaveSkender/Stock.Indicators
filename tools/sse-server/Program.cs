using System.Globalization;
using System.Text.Json;
using Skender.Stock.Indicators;
using Test.Data;
using TestData = Test.Data.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add native .NET 10 OpenAPI support
builder.Services.AddOpenApi(options => {
    options.AddDocumentTransformer((document, _, _) => {
        document.Info = new() {
            Title = "SSE Quote Server",
            Version = "v1",
            Description = "Server-Sent Events (SSE) endpoint for streaming quote data with configurable delivery rate and time intervals."
        };

        return Task.CompletedTask;
    });
});

WebApplication app = builder.Build();

// Configure JSON serialization options
JsonSerializerOptions jsonOptions = new() {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// Streams randomly generated quote data via Server-Sent Events (SSE)
app.MapGet("/quotes/random", async (
    HttpContext context,
    int interval = 100,
    int? batchSize = null,
    string quoteInterval = "1m"
) => {
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
    TimeSpan timestampIncrement = ParseInterval(quoteInterval);

    Console.WriteLine(
        $"[Random] Starting stream - delivery: {interval}ms, quoteInterval: {quoteInterval}, batchSize: {batchSize?.ToString(CultureInfo.InvariantCulture) ?? "unlimited"}");

    // Use Test.Data.RandomGbm for random quote generation
    PeriodSize periodSize = Test.SseServer.Utilities.ParseQuoteIntervalToPeriodSize(quoteInterval);
    RandomGbm generator = new(bars: 0, seed: 1000.0, periodSize: periodSize);
    DateTime currentTimestamp = DateTime.UtcNow.AddMinutes(-1000);

    try
    {
        while (!context.RequestAborted.IsCancellationRequested)
        {
            // Generate next random quote
            generator.Add(currentTimestamp);
            Quote quote = generator[^1];

            // Serialize quote as JSON
            string json = JsonSerializer.Serialize(quote, jsonOptions);

            // Write SSE event manually
            string sseData = $"event: quote\ndata: {json}\n\n";
            await context.Response
                .WriteAsync(sseData, context.RequestAborted)
                .ConfigureAwait(false);
            await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);

            delivered++;
            currentTimestamp = currentTimestamp.Add(timestampIncrement);

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
})
.WithName("GetRandomQuotes")
.WithTags("Quotes")
.AddOpenApiOperationTransformer((operation, _, _) => {
    // Add regex pattern validation for quoteInterval parameter
    Microsoft.OpenApi.IOpenApiParameter? quoteIntervalParam = operation.Parameters?.FirstOrDefault(p => p.Name == "quoteInterval");
    if (quoteIntervalParam is Microsoft.OpenApi.OpenApiParameter concreteParam && concreteParam.Schema is Microsoft.OpenApi.OpenApiSchema schema)
    {
        schema.Pattern = @"^\d+(\.\d+)?(s|sec|second|seconds|m|min|minute|minutes|h|hr|hour|hours|d|day|days)$";
    }

    return Task.CompletedTask;
});

// Streams historical quote data from the longest available dataset via Server-Sent Events (SSE)
app.MapGet("/quotes/longest", async (
    HttpContext context,
    int interval = 100,
    int? batchSize = null,
    string quoteInterval = "1m",
    string? scenario = null
) => {
    // Validate interval parameter
    if (interval <= 0)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Interval must be greater than 0").ConfigureAwait(false);
        return;
    }

    context.Response.ContentType = "text/event-stream";
    context.Response.Headers.CacheControl = "no-cache";

    IReadOnlyList<Quote> longestQuotes = TestData.GetLongest();

    // Guard against empty data
    if (longestQuotes == null || longestQuotes.Count == 0)
    {
        Console.WriteLine("[Longest] ERROR: No quote data available from TestData.GetLongest()");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Server error: No quote data available").ConfigureAwait(false);
        return;
    }

    int totalQuotes = batchSize ?? longestQuotes.Count;
    int delivered = 0;
    TimeSpan timestampIncrement = ParseInterval(quoteInterval);
    DateTime baseTimestamp = longestQuotes[0].Timestamp;
    List<Quote>? streamedQuotes = scenario is null ? null : [];

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

            if (streamedQuotes is not null)
            {
                streamedQuotes.Add(quote);
            }

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

        if (streamedQuotes is not null)
        {
            await SendScenarioEvents(context, scenario, streamedQuotes, interval, jsonOptions).ConfigureAwait(false);
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
})
.WithName("GetLongestQuotes")
.WithTags("Quotes")
.AddOpenApiOperationTransformer((operation, _, _) => {
    // Add regex pattern validation for quoteInterval parameter
    Microsoft.OpenApi.IOpenApiParameter? quoteIntervalParam = operation.Parameters?.FirstOrDefault(p => p.Name == "quoteInterval");
    if (quoteIntervalParam is Microsoft.OpenApi.OpenApiParameter concreteParam && concreteParam.Schema is Microsoft.OpenApi.OpenApiSchema schema)
    {
        schema.Pattern = @"^\d+(\.\d+)?(s|sec|second|seconds|m|min|minute|minutes|h|hr|hour|hours|d|day|days)$";
    }

    return Task.CompletedTask;
});

Console.WriteLine("SSE Server starting...");
Console.WriteLine("Endpoints:");
Console.WriteLine("  - /quotes/random?interval=100&batchSize=1000&quoteInterval=1h");
Console.WriteLine("  - /quotes/longest?interval=100&batchSize=1000&quoteInterval=5m");
Console.WriteLine("  - /openapi/v1.json for OpenAPI JSON specification");

// Map OpenAPI endpoint (dev environment only for security)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.RunAsync().ConfigureAwait(false);

#pragma warning disable CS1587, RCS1263 // XML comment warnings for local function
/// <summary>
/// Parse interval string (e.g., "1h", "5m", "30s") to TimeSpan
/// </summary>
/// <param name="intervalString">Time interval string</param>
/// <returns>Parsed TimeSpan or 1 minute if parsing fails</returns>
static TimeSpan ParseInterval(string intervalString)
#pragma warning restore CS1587, RCS1263
{
    if (string.IsNullOrWhiteSpace(intervalString))
    {
        return TimeSpan.FromMinutes(1);
    }

    string trimmed = intervalString.Trim().ToLowerInvariant();

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

static async Task SendScenarioEvents(
    HttpContext context,
    string? scenario,
    IReadOnlyList<Quote> streamedQuotes,
    int interval,
    JsonSerializerOptions jsonOptions)
{
    if (string.IsNullOrWhiteSpace(scenario))
    {
        return;
    }

    List<SseQuoteAction> actions = scenario switch {
        "stc-rollbacks" => BuildStcRollbackActions(streamedQuotes),
        "allhubs-rollbacks" => BuildAllHubsRollbackActions(streamedQuotes),
        _ => []
    };

    if (actions.Count == 0)
    {
        Console.WriteLine($"[Longest] No scenario actions for '{scenario}'");
        return;
    }

    Console.WriteLine($"[Longest] Sending {actions.Count} scenario events for '{scenario}'");

    foreach (SseQuoteAction action in actions)
    {
        string json = JsonSerializer.Serialize(action.Payload, jsonOptions);
        string sseData = $"event: {action.EventType}\ndata: {json}\n\n";
        await context.Response
            .WriteAsync(sseData, context.RequestAborted)
            .ConfigureAwait(false);
        await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
        await Task.Delay(interval, context.RequestAborted).ConfigureAwait(false);
    }
}

static List<SseQuoteAction> BuildStcRollbackActions(IReadOnlyList<Quote> streamedQuotes)
{
    List<SseQuoteAction> actions = [];

    if (streamedQuotes.Count > 80)
    {
        actions.Add(SseQuoteAction.Add(streamedQuotes[80]));
    }

    if (streamedQuotes.Count > 100)
    {
        Quote rebuildQuote = streamedQuotes[100];
        actions.Add(SseQuoteAction.Remove(100, rebuildQuote));
        actions.Add(SseQuoteAction.Add(rebuildQuote));
    }

    if (streamedQuotes.Count > 500)
    {
        actions.Add(SseQuoteAction.Add(streamedQuotes[500]));
    }

    return actions;
}

static List<SseQuoteAction> BuildAllHubsRollbackActions(IReadOnlyList<Quote> streamedQuotes)
{
    List<SseQuoteAction> actions = [];

    if (streamedQuotes.Count > 10)
    {
        actions.Add(SseQuoteAction.Add(streamedQuotes[10]));
    }

    if (streamedQuotes.Count > 100)
    {
        Quote rebuildQuote = streamedQuotes[100];
        actions.Add(SseQuoteAction.Remove(100, rebuildQuote));
        actions.Add(SseQuoteAction.Add(rebuildQuote));
    }

    if (streamedQuotes.Count > 1600)
    {
        Quote original = streamedQuotes[1600];
        Quote replacementQuote = new(
            original.Timestamp,
            original.Open,
            original.High,
            original.Low,
            original.Close * 1.01m,
            original.Volume);
        actions.Add(SseQuoteAction.Add(replacementQuote));
    }

    if (streamedQuotes.Count > 0)
    {
        Quote lastQuote = streamedQuotes[^1];
        actions.Add(SseQuoteAction.Add(new Quote(
            lastQuote.Timestamp,
            lastQuote.Open,
            lastQuote.High,
            lastQuote.Low,
            lastQuote.Close * 0.99m,
            lastQuote.Volume)));
        actions.Add(SseQuoteAction.Add(new Quote(
            lastQuote.Timestamp,
            lastQuote.Open,
            lastQuote.High,
            lastQuote.Low,
            lastQuote.Close * 1.01m,
            lastQuote.Volume)));
        actions.Add(SseQuoteAction.Add(lastQuote));
    }

    return actions;
}

record SseQuoteAction(string EventType, QuoteAction Payload)
{
    public static SseQuoteAction Add(Quote quote)
        => new("add", new QuoteAction(quote, null));

    public static SseQuoteAction Remove(int cacheIndex, Quote quote)
        => new("remove", new QuoteAction(quote, cacheIndex));
}

record QuoteAction(Quote Quote, int? CacheIndex);
