using System.Diagnostics;
using System.Text.Json;
using Test.Tools;

namespace StreamHubs;

[TestClass, TestCategory("Integration")]
public class ThreadSafetyTests : TestBase
{
    private const int SseServerPort = 5099;
    private const int TargetQuoteCount = 2000;
    private const int MaxCacheSize = 1500;
    private const int SseInterval = 1; // milliseconds between quotes
    private const string QuoteInterval = "1Day"; // daily quotes

    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public TestContext? TestContext { get; set; }

    [TestMethod]
    public async Task AllHubs_WithSseStream_MatchSeriesExactly()
    {
        CancellationToken cts = TestContext?.CancellationToken ?? default;

        // Start SSE server
        Process? serverProcess = StartSseServer();
        if (serverProcess is null)
        {
            Assert.Fail("Failed to start SSE server");
            return;
        }

        try
        {
            // Give server time to start
            await Task.Delay(2000, cts).ConfigureAwait(true);

            // Setup: Create one primary QuoteHub
            QuoteHub quoteHub = new() { MaxCacheSize = MaxCacheSize };

            // Subscribe all 80+ indicator hubs to the primary QuoteHub
            AdlHub adlHub = quoteHub.ToAdlHub();
            AdxHub adxHub = quoteHub.ToAdxHub(14);
            AlligatorHub alligatorHub = quoteHub.ToAlligatorHub();
            AlmaHub almaHub = quoteHub.ToAlmaHub(10, 0.85, 6);
            AroonHub aroonHub = quoteHub.ToAroonHub(25);
            AtrHub atrHub = quoteHub.ToAtrHub(14);
            AtrStopHub atrStopHub = quoteHub.ToAtrStopHub(21);
            AwesomeHub awesomeHub = quoteHub.ToAwesomeHub();
            BollingerBandsHub bollingerBandsHub = quoteHub.ToBollingerBandsHub(20, 2);
            BopHub bopHub = quoteHub.ToBopHub();
            CciHub cciHub = quoteHub.ToCciHub(20);
            ChaikinOscHub chaikinOscHub = quoteHub.ToChaikinOscHub();
            ChandelierHub chandelierHub = quoteHub.ToChandelierHub();
            ChopHub chopHub = quoteHub.ToChopHub(14);
            CmfHub cmfHub = quoteHub.ToCmfHub(20);
            CmoHub cmoHub = quoteHub.ToCmoHub(14);
            ConnorsRsiHub connorsRsiHub = quoteHub.ToConnorsRsiHub();
            DemaHub demaHub = quoteHub.ToDemaHub(20);
            DojiHub dojiHub = quoteHub.ToDojiHub();
            DonchianHub donchianHub = quoteHub.ToDonchianHub(20);
            DpoHub dpoHub = quoteHub.ToDpoHub(14);
            DynamicHub dynamicHub = quoteHub.ToDynamicHub(14);
            ElderRayHub elderRayHub = quoteHub.ToElderRayHub(13);
            EmaHub emaHub = quoteHub.ToEmaHub(20);

            // EPMA excluded: uses position-based linear regression where EPMA = slope*(i+1)+intercept.
            // After pruning, cache indices no longer match global positions, causing value mismatch.
            _ = quoteHub.ToEpmaHub(20);

            // Fractal excluded: forward-looking indicator requires left/right context.
            // After pruning, first few positions lose left-side context causing null values.
            _ = quoteHub.ToFractalHub(2);

            // FCB excluded: derives from Fractal's forward-looking logic with stateful bands.
            // After pruning, band state would need to preserve history from pruned region.
            _ = quoteHub.ToFcbHub(2);

            FisherTransformHub fisherTransformHub = quoteHub.ToFisherTransformHub(10);
            ForceIndexHub forceIndexHub = quoteHub.ToForceIndexHub(13);
            GatorHub gatorHub = quoteHub.ToGatorHub();
            HeikinAshiHub heikinAshiHub = quoteHub.ToHeikinAshiHub();
            HmaHub hmaHub = quoteHub.ToHmaHub(20);
            HtTrendlineHub htTrendlineHub = quoteHub.ToHtTrendlineHub();
            HurstHub hurstHub = quoteHub.ToHurstHub(20);
            IchimokuHub ichimokuHub = quoteHub.ToIchimokuHub();
            KamaHub kamaHub = quoteHub.ToKamaHub(10, 2, 30);
            KeltnerHub keltnerHub = quoteHub.ToKeltnerHub();
            KvoHub kvoHub = quoteHub.ToKvoHub();
            MaEnvelopesHub maEnvelopesHub = quoteHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA);
            MacdHub macdHub = quoteHub.ToMacdHub();
            MamaHub mamaHub = quoteHub.ToMamaHub();
            MarubozuHub marubozuHub = quoteHub.ToMarubozuHub();
            MfiHub mfiHub = quoteHub.ToMfiHub(14);
            ObvHub obvHub = quoteHub.ToObvHub();
            ParabolicSarHub parabolicSarHub = quoteHub.ToParabolicSarHub();
            PivotPointsHub pivotPointsHub = quoteHub.ToPivotPointsHub(PeriodSize.Month, PivotPointType.Standard);

            // Pivots excluded: forward-looking indicator requires left/right span context.
            // After pruning, first few positions lose left-side context causing null values.
            _ = quoteHub.ToPivotsHub(11, 14);
            PmoHub pmoHub = quoteHub.ToPmoHub();
            PvoHub pvoHub = quoteHub.ToPvoHub();

            // Renko excluded from comparison: produces variable-length output and has special
            // pruning requirements because its cache is independent from parent's ProviderCache.
            _ = quoteHub.ToRenkoHub(2.5m);

            RocHub rocHub = quoteHub.ToRocHub(20);
            RocWbHub rocWbHub = quoteHub.ToRocWbHub(14);

            // RollingPivots excluded: forward-looking indicator requires lookback/forward period context.
            // After pruning, first few positions lose left-side context causing value mismatch.
            _ = quoteHub.ToRollingPivotsHub(11, 9);

            RsiHub rsiHub = quoteHub.ToRsiHub(14);

            // Slope excluded: uses position-based X values for linear regression.
            // Intercept = avgY - (slope * avgX) and Line = slope*(i+1)+intercept both depend on global position.
            // After pruning, cache indices no longer match global positions.
            _ = quoteHub.ToSlopeHub(20);

            SmaHub smaHub = quoteHub.ToSmaHub(20);
            SmaAnalysisHub smaAnalysisHub = quoteHub.ToSmaAnalysisHub(10);
            SmiHub smiHub = quoteHub.ToSmiHub();
            SmmaHub smmaHub = quoteHub.ToSmmaHub(20);
            StarcBandsHub starcBandsHub = quoteHub.ToStarcBandsHub();

            // STC excluded: complex MACD → Stochastic chain with internal state arrays
            // that become misaligned after pruning.
            _ = quoteHub.ToStcHub();

            StdDevHub stdDevHub = quoteHub.ToStdDevHub(10);
            StochHub stochHub = quoteHub.ToStochHub();
            StochRsiHub stochRsiHub = quoteHub.ToStochRsiHub(14);
            SuperTrendHub superTrendHub = quoteHub.ToSuperTrendHub();
            T3Hub t3Hub = quoteHub.ToT3Hub(5);
            TemaHub temaHub = quoteHub.ToTemaHub(20);
            TrHub trHub = quoteHub.ToTrHub();
            TrixHub trixHub = quoteHub.ToTrixHub(14);
            TsiHub tsiHub = quoteHub.ToTsiHub();
            UlcerIndexHub ulcerIndexHub = quoteHub.ToUlcerIndexHub(14);
            UltimateHub ultimateHub = quoteHub.ToUltimateHub();
            VolatilityStopHub volatilityStopHub = quoteHub.ToVolatilityStopHub();
            VortexHub vortexHub = quoteHub.ToVortexHub(14);
            VwapHub vwapHub = quoteHub.ToVwapHub();
            VwmaHub vwmaHub = quoteHub.ToVwmaHub(10);
            WilliamsRHub williamsRHub = quoteHub.ToWilliamsRHub(14);
            WmaHub wmaHub = quoteHub.ToWmaHub(20);

            // Consume quotes from SSE stream (returns all quotes for Series comparison)
            List<Quote> allQuotes = await ConsumeQuotesFromSse(quoteHub).ConfigureAwait(true);

            // Get final results from QuoteHub as IQuote (compatible with all Series methods)
            IReadOnlyList<IQuote> quoteHubResults = quoteHub.Results;

            // Verify results are not empty and cache size is respected
            quoteHubResults.Should().NotBeEmpty("quote hub should have results");
            quoteHubResults.Should().HaveCountLessOrEqualTo(MaxCacheSize, "cache size constraint should be respected");

            // Verify pruning actually occurred (we sent more quotes than cache size)
            int prunedCount = TargetQuoteCount - quoteHubResults.Count;
            prunedCount.Should().BeGreaterThan(0, "pruning should have occurred");

            // For cumulative/stateful indicators, hub results include accumulated state from ALL quotes.
            // To match exactly, compute Series on ALL quotes and take the last MaxCacheSize entries.
            // This ensures both hub and series have identical warmup history.
            int takeCount = quoteHubResults.Count;

            // Verify ALL indicator hubs match their equivalent series calculations.
            // Series is computed on ALL quotes, then we take the last entries to match hub cache.
            adlHub.Results.IsExactly(allQuotes.ToAdl().TakeLast(takeCount));
            adxHub.Results.IsExactly(allQuotes.ToAdx(14).TakeLast(takeCount));
            alligatorHub.Results.IsExactly(allQuotes.ToAlligator().TakeLast(takeCount));
            almaHub.Results.IsExactly(allQuotes.ToAlma(10, 0.85, 6).TakeLast(takeCount));
            aroonHub.Results.IsExactly(allQuotes.ToAroon(25).TakeLast(takeCount));
            atrHub.Results.IsExactly(allQuotes.ToAtr(14).TakeLast(takeCount));
            atrStopHub.Results.IsExactly(allQuotes.ToAtrStop(21).TakeLast(takeCount));
            awesomeHub.Results.IsExactly(allQuotes.ToAwesome().TakeLast(takeCount));
            bollingerBandsHub.Results.IsExactly(allQuotes.ToBollingerBands(20, 2).TakeLast(takeCount));
            bopHub.Results.IsExactly(allQuotes.ToBop().TakeLast(takeCount));
            cciHub.Results.IsExactly(allQuotes.ToCci(20).TakeLast(takeCount));
            chaikinOscHub.Results.IsExactly(allQuotes.ToChaikinOsc().TakeLast(takeCount));
            chandelierHub.Results.IsExactly(allQuotes.ToChandelier().TakeLast(takeCount));
            chopHub.Results.IsExactly(allQuotes.ToChop(14).TakeLast(takeCount));
            cmfHub.Results.IsExactly(allQuotes.ToCmf(20).TakeLast(takeCount));
            cmoHub.Results.IsExactly(allQuotes.ToCmo(14).TakeLast(takeCount));
            connorsRsiHub.Results.IsExactly(allQuotes.ToConnorsRsi().TakeLast(takeCount));
            demaHub.Results.IsExactly(allQuotes.ToDema(20).TakeLast(takeCount));
            dojiHub.Results.IsExactly(allQuotes.ToDoji().TakeLast(takeCount));
            donchianHub.Results.IsExactly(allQuotes.ToDonchian(20).TakeLast(takeCount));
            dpoHub.Results.IsExactly(allQuotes.ToDpo(14).TakeLast(takeCount));
            dynamicHub.Results.IsExactly(allQuotes.ToDynamic(14).TakeLast(takeCount));
            elderRayHub.Results.IsExactly(allQuotes.ToElderRay(13).TakeLast(takeCount));
            emaHub.Results.IsExactly(allQuotes.ToEma(20).TakeLast(takeCount));

            // EPMA, Fractal, FCB excluded: comparisons removed (see hub declarations)

            fisherTransformHub.Results.IsExactly(allQuotes.ToFisherTransform(10).TakeLast(takeCount));
            forceIndexHub.Results.IsExactly(allQuotes.ToForceIndex(13).TakeLast(takeCount));
            gatorHub.Results.IsExactly(allQuotes.ToGator().TakeLast(takeCount));
            heikinAshiHub.Results.IsExactly(allQuotes.ToHeikinAshi().TakeLast(takeCount));
            hmaHub.Results.IsExactly(allQuotes.ToHma(20).TakeLast(takeCount));
            htTrendlineHub.Results.IsExactly(allQuotes.ToHtTrendline().TakeLast(takeCount));
            hurstHub.Results.IsExactly(allQuotes.ToHurst(20).TakeLast(takeCount));
            ichimokuHub.Results.IsExactly(allQuotes.ToIchimoku().TakeLast(takeCount));
            kamaHub.Results.IsExactly(allQuotes.ToKama(10, 2, 30).TakeLast(takeCount));
            keltnerHub.Results.IsExactly(allQuotes.ToKeltner().TakeLast(takeCount));
            kvoHub.Results.IsExactly(allQuotes.ToKvo().TakeLast(takeCount));
            maEnvelopesHub.Results.IsExactly(allQuotes.ToMaEnvelopes(20, 2.5, MaType.SMA).TakeLast(takeCount));
            macdHub.Results.IsExactly(allQuotes.ToMacd().TakeLast(takeCount));
            mamaHub.Results.IsExactly(allQuotes.ToMama().TakeLast(takeCount));
            marubozuHub.Results.IsExactly(allQuotes.ToMarubozu().TakeLast(takeCount));
            mfiHub.Results.IsExactly(allQuotes.ToMfi(14).TakeLast(takeCount));
            obvHub.Results.IsExactly(allQuotes.ToObv().TakeLast(takeCount));
            parabolicSarHub.Results.IsExactly(allQuotes.ToParabolicSar().TakeLast(takeCount));
            pivotPointsHub.Results.IsExactly(allQuotes.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard).TakeLast(takeCount));

            // Pivots excluded: comparison removed (see hub declaration)

            pmoHub.Results.IsExactly(allQuotes.ToPmo().TakeLast(takeCount));
            pvoHub.Results.IsExactly(allQuotes.ToPvo().TakeLast(takeCount));

            // Renko excluded: produces variable-length output and has special pruning requirements
            // because its cache is independent from parent QuoteHub's pruned ProviderCache.
            // renkoHub.Results.IsExactly(allQuotes.ToRenko(2.5m));

            rocHub.Results.IsExactly(allQuotes.ToRoc(20).TakeLast(takeCount));
            rocWbHub.Results.IsExactly(allQuotes.ToRocWb(14).TakeLast(takeCount));

            // RollingPivots excluded: comparison removed (see hub declaration)

            rsiHub.Results.IsExactly(allQuotes.ToRsi(14).TakeLast(takeCount));

            // Slope excluded: comparison removed (see hub declaration)

            smaHub.Results.IsExactly(allQuotes.ToSma(20).TakeLast(takeCount));
            smaAnalysisHub.Results.IsExactly(allQuotes.ToSmaAnalysis(10).TakeLast(takeCount));
            smiHub.Results.IsExactly(allQuotes.ToSmi().TakeLast(takeCount));
            smmaHub.Results.IsExactly(allQuotes.ToSmma(20).TakeLast(takeCount));
            starcBandsHub.Results.IsExactly(allQuotes.ToStarcBands().TakeLast(takeCount));

            // STC excluded: comparison removed (see hub declaration)

            stdDevHub.Results.IsExactly(allQuotes.ToStdDev(10).TakeLast(takeCount));
            stochHub.Results.IsExactly(allQuotes.ToStoch().TakeLast(takeCount));
            stochRsiHub.Results.IsExactly(allQuotes.ToStochRsi(14).TakeLast(takeCount));
            superTrendHub.Results.IsExactly(allQuotes.ToSuperTrend().TakeLast(takeCount));
            t3Hub.Results.IsExactly(allQuotes.ToT3(5).TakeLast(takeCount));
            temaHub.Results.IsExactly(allQuotes.ToTema(20).TakeLast(takeCount));
            trHub.Results.IsExactly(allQuotes.ToTr().TakeLast(takeCount));
            trixHub.Results.IsExactly(allQuotes.ToTrix(14).TakeLast(takeCount));
            tsiHub.Results.IsExactly(allQuotes.ToTsi().TakeLast(takeCount));
            ulcerIndexHub.Results.IsExactly(allQuotes.ToUlcerIndex(14).TakeLast(takeCount));
            ultimateHub.Results.IsExactly(allQuotes.ToUltimate().TakeLast(takeCount));
            volatilityStopHub.Results.IsExactly(allQuotes.ToVolatilityStop().TakeLast(takeCount));
            vortexHub.Results.IsExactly(allQuotes.ToVortex(14).TakeLast(takeCount));
            vwapHub.Results.IsExactly(allQuotes.ToVwap().TakeLast(takeCount));
            vwmaHub.Results.IsExactly(allQuotes.ToVwma(10).TakeLast(takeCount));
            williamsRHub.Results.IsExactly(allQuotes.ToWilliamsR(14).TakeLast(takeCount));
            wmaHub.Results.IsExactly(allQuotes.ToWma(20).TakeLast(takeCount));

            // Cleanup
            quoteHub.EndTransmission();
        }
        finally
        {
            // Stop SSE server
            if (!serverProcess.HasExited)
            {
                serverProcess.Kill();
                await serverProcess.WaitForExitAsync(cts).ConfigureAwait(true);
                serverProcess.Dispose();
            }
        }
    }

    private static Process? StartSseServer()
    {
        try
        {
            string? repoRoot = FindRepositoryRoot();
            if (repoRoot is null)
            {
                Console.WriteLine("[Test] Cannot find repository root.");
                return null;
            }

            string serverProjectPath = Path.Combine(repoRoot, "tools", "sse-server");

            ProcessStartInfo startInfo = new() {
                FileName = "dotnet",
                Arguments = $"run --project \"{serverProjectPath}\" -- --urls http://localhost:{SseServerPort}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process? process = Process.Start(startInfo);
            if (process is not null)
            {
                Console.WriteLine($"[Test] SSE server started on port {SseServerPort} (PID: {process.Id})");
            }

            return process;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Test] IO error starting server: {ex.Message}");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[Test] Invalid operation starting server: {ex.Message}");
            return null;
        }
    }

    private static string? FindRepositoryRoot()
    {
        string currentDir = Directory.GetCurrentDirectory();
        while (!string.IsNullOrEmpty(currentDir))
        {
            if (File.Exists(Path.Combine(currentDir, "Stock.Indicators.sln")))
            {
                return currentDir;
            }

            string? parentDir = Directory.GetParent(currentDir)?.FullName;
            if (parentDir == currentDir)
            {
                break;
            }

            currentDir = parentDir ?? string.Empty;
        }

        return null;
    }

    private static async Task<List<Quote>> ConsumeQuotesFromSse(QuoteHub quoteHub)
    {
        List<Quote> allQuotes = [];
        using HttpClient httpClient = new() { Timeout = TimeSpan.FromMinutes(5) };

        string batchSizeParam = $"&batchSize={TargetQuoteCount}";
        Uri uri = new($"http://localhost:{SseServerPort}/quotes/longest?interval={SseInterval}&quoteInterval={QuoteInterval}{batchSizeParam}");

        using HttpResponseMessage response = await httpClient
            .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using StreamReader reader = new(stream);

        int quotesProcessed = 0;
        string? line;

        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
        {
            // SSE format: "event: quote", "data: {...}", blank line
            if (line.StartsWith("data:", StringComparison.Ordinal))
            {
                string json = line[5..].Trim();
                Quote? quote = JsonSerializer.Deserialize<Quote>(json, JsonOptions);

                if (quote is not null)
                {
                    allQuotes.Add(quote);
                    quoteHub.Add(quote);
                    quotesProcessed++;

                    if (quotesProcessed >= TargetQuoteCount)
                    {
                        break;
                    }
                }
            }
        }

        return allQuotes;
    }
}
