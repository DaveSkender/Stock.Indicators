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
            EpmaHub epmaHub = quoteHub.ToEpmaHub(20);
            FcbHub fcbHub = quoteHub.ToFcbHub(2);
            FisherTransformHub fisherTransformHub = quoteHub.ToFisherTransformHub(10);
            ForceIndexHub forceIndexHub = quoteHub.ToForceIndexHub(13);
            FractalHub fractalHub = quoteHub.ToFractalHub(2);
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
            PivotsHub pivotsHub = quoteHub.ToPivotsHub(11, 14);
            PmoHub pmoHub = quoteHub.ToPmoHub();
            PvoHub pvoHub = quoteHub.ToPvoHub();
            RenkoHub renkoHub = quoteHub.ToRenkoHub(2.5m);
            RocHub rocHub = quoteHub.ToRocHub(20);
            RocWbHub rocWbHub = quoteHub.ToRocWbHub(14);
            RollingPivotsHub rollingPivotsHub = quoteHub.ToRollingPivotsHub(11, 9);
            RsiHub rsiHub = quoteHub.ToRsiHub(14);
            SlopeHub slopeHub = quoteHub.ToSlopeHub(20);
            SmaHub smaHub = quoteHub.ToSmaHub(20);
            SmaAnalysisHub smaAnalysisHub = quoteHub.ToSmaAnalysisHub(10);
            SmiHub smiHub = quoteHub.ToSmiHub();
            SmmaHub smmaHub = quoteHub.ToSmmaHub(20);
            StarcBandsHub starcBandsHub = quoteHub.ToStarcBandsHub();
            StcHub stcHub = quoteHub.ToStcHub();
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

            // Consume quotes from SSE stream
            await ConsumeQuotesFromSse(quoteHub).ConfigureAwait(true);

            // Get final results from QuoteHub as IQuote (compatible with all Series methods)
            IReadOnlyList<IQuote> quoteHubResults = quoteHub.Results;

            // Verify results are not empty and cache size is respected
            quoteHubResults.Should().NotBeEmpty("quote hub should have results");
            quoteHubResults.Should().HaveCountLessOrEqualTo(MaxCacheSize, "cache size constraint should be respected");

            // Verify ALL indicator hubs match their equivalent series calculations exactly
            adlHub.Results.IsExactly(quoteHubResults.ToAdl());
            adxHub.Results.IsExactly(quoteHubResults.ToAdx(14));
            alligatorHub.Results.IsExactly(quoteHubResults.ToAlligator());
            almaHub.Results.IsExactly(quoteHubResults.ToAlma(10, 0.85, 6));
            aroonHub.Results.IsExactly(quoteHubResults.ToAroon(25));
            atrHub.Results.IsExactly(quoteHubResults.ToAtr(14));
            atrStopHub.Results.IsExactly(quoteHubResults.ToAtrStop(21));
            awesomeHub.Results.IsExactly(quoteHubResults.ToAwesome());
            bollingerBandsHub.Results.IsExactly(quoteHubResults.ToBollingerBands(20, 2));
            bopHub.Results.IsExactly(quoteHubResults.ToBop());
            cciHub.Results.IsExactly(quoteHubResults.ToCci(20));
            chaikinOscHub.Results.IsExactly(quoteHubResults.ToChaikinOsc());
            chandelierHub.Results.IsExactly(quoteHubResults.ToChandelier());
            chopHub.Results.IsExactly(quoteHubResults.ToChop(14));
            cmfHub.Results.IsExactly(quoteHubResults.ToCmf(20));
            cmoHub.Results.IsExactly(quoteHubResults.ToCmo(14));
            connorsRsiHub.Results.IsExactly(quoteHubResults.ToConnorsRsi());
            demaHub.Results.IsExactly(quoteHubResults.ToDema(20));
            dojiHub.Results.IsExactly(quoteHubResults.ToDoji());
            donchianHub.Results.IsExactly(quoteHubResults.ToDonchian(20));
            dpoHub.Results.IsExactly(quoteHubResults.ToDpo(14));
            dynamicHub.Results.IsExactly(quoteHubResults.ToDynamic(14));
            elderRayHub.Results.IsExactly(quoteHubResults.ToElderRay(13));
            emaHub.Results.IsExactly(quoteHubResults.ToEma(20));
            epmaHub.Results.IsExactly(quoteHubResults.ToEpma(20));
            fcbHub.Results.IsExactly(quoteHubResults.ToFcb(2));
            fisherTransformHub.Results.IsExactly(quoteHubResults.ToFisherTransform(10));
            forceIndexHub.Results.IsExactly(quoteHubResults.ToForceIndex(13));
            fractalHub.Results.IsExactly(quoteHubResults.ToFractal(2));
            gatorHub.Results.IsExactly(quoteHubResults.ToGator());
            heikinAshiHub.Results.IsExactly(quoteHubResults.ToHeikinAshi());
            hmaHub.Results.IsExactly(quoteHubResults.ToHma(20));
            htTrendlineHub.Results.IsExactly(quoteHubResults.ToHtTrendline());
            hurstHub.Results.IsExactly(quoteHubResults.ToHurst(20));
            ichimokuHub.Results.IsExactly(quoteHubResults.ToIchimoku());
            kamaHub.Results.IsExactly(quoteHubResults.ToKama(10, 2, 30));
            keltnerHub.Results.IsExactly(quoteHubResults.ToKeltner());
            kvoHub.Results.IsExactly(quoteHubResults.ToKvo());
            maEnvelopesHub.Results.IsExactly(quoteHubResults.ToMaEnvelopes(20, 2.5, MaType.SMA));
            macdHub.Results.IsExactly(quoteHubResults.ToMacd());
            mamaHub.Results.IsExactly(quoteHubResults.ToMama());
            marubozuHub.Results.IsExactly(quoteHubResults.ToMarubozu());
            mfiHub.Results.IsExactly(quoteHubResults.ToMfi(14));
            obvHub.Results.IsExactly(quoteHubResults.ToObv());
            parabolicSarHub.Results.IsExactly(quoteHubResults.ToParabolicSar());
            pivotPointsHub.Results.IsExactly(quoteHubResults.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard));
            pivotsHub.Results.IsExactly(quoteHubResults.ToPivots(11, 14));
            pmoHub.Results.IsExactly(quoteHubResults.ToPmo());
            pvoHub.Results.IsExactly(quoteHubResults.ToPvo());
            renkoHub.Results.IsExactly(quoteHubResults.ToRenko(2.5m));
            rocHub.Results.IsExactly(quoteHubResults.ToRoc(20));
            rocWbHub.Results.IsExactly(quoteHubResults.ToRocWb(14));
            rollingPivotsHub.Results.IsExactly(quoteHubResults.ToRollingPivots(11, 9));
            rsiHub.Results.IsExactly(quoteHubResults.ToRsi(14));
            slopeHub.Results.IsExactly(quoteHubResults.ToSlope(20));
            smaHub.Results.IsExactly(quoteHubResults.ToSma(20));
            smaAnalysisHub.Results.IsExactly(quoteHubResults.ToSmaAnalysis(10));
            smiHub.Results.IsExactly(quoteHubResults.ToSmi());
            smmaHub.Results.IsExactly(quoteHubResults.ToSmma(20));
            starcBandsHub.Results.IsExactly(quoteHubResults.ToStarcBands());
            stcHub.Results.IsExactly(quoteHubResults.ToStc());
            stdDevHub.Results.IsExactly(quoteHubResults.ToStdDev(10));
            stochHub.Results.IsExactly(quoteHubResults.ToStoch());
            stochRsiHub.Results.IsExactly(quoteHubResults.ToStochRsi(14));
            superTrendHub.Results.IsExactly(quoteHubResults.ToSuperTrend());
            t3Hub.Results.IsExactly(quoteHubResults.ToT3(5));
            temaHub.Results.IsExactly(quoteHubResults.ToTema(20));
            trHub.Results.IsExactly(quoteHubResults.ToTr());
            trixHub.Results.IsExactly(quoteHubResults.ToTrix(14));
            tsiHub.Results.IsExactly(quoteHubResults.ToTsi());
            ulcerIndexHub.Results.IsExactly(quoteHubResults.ToUlcerIndex(14));
            ultimateHub.Results.IsExactly(quoteHubResults.ToUltimate());
            volatilityStopHub.Results.IsExactly(quoteHubResults.ToVolatilityStop());
            vortexHub.Results.IsExactly(quoteHubResults.ToVortex(14));
            vwapHub.Results.IsExactly(quoteHubResults.ToVwap());
            vwmaHub.Results.IsExactly(quoteHubResults.ToVwma(10));
            williamsRHub.Results.IsExactly(quoteHubResults.ToWilliamsR(14));
            wmaHub.Results.IsExactly(quoteHubResults.ToWma(20));

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

    private static async Task ConsumeQuotesFromSse(QuoteHub quoteHub)
    {
        using HttpClient httpClient = new() { Timeout = TimeSpan.FromMinutes(5) };

        string batchSizeParam = $"&batchSize={TargetQuoteCount}";
        Uri uri = new($"http://localhost:{SseServerPort}?interval={SseInterval}&quoteInterval={QuoteInterval}{batchSizeParam}");

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
                    quoteHub.Add(quote);
                    quotesProcessed++;

                    if (quotesProcessed >= TargetQuoteCount)
                    {
                        break;
                    }
                }
            }
        }
    }
}
