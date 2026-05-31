using Test.Tools;

namespace StressLoading;

[TestClass, TestCategory("Integration")]
public class StreamLoadTests
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();

    [TestMethod]
    public void StreamMany() // from quote provider
    {
        /******************************************************
         * Attaches many stream observers to one Quote provider
         * for a full sprectrum stream collective.
         *
         * Currently, it does not include any [direct] chains.
         *
         * This test covers most of the unusual test cases, like:
         *
         *  - out of order quotes (late arrivals)
         *  - duplicates, but not to an overflow situation
         *
         ******************************************************/

        int length = quotes.Count;

        // setup quote hub
        QuoteHub quoteHub = new();

        // initialize observers
        AdlHub adlHub = quoteHub.ToAdlHub();
        AlligatorHub alligatorHub = quoteHub.ToAlligatorHub();
        AtrHub atrHub = quoteHub.ToAtrHub();
        AtrStopHub atrStopHub = quoteHub.ToAtrStopHub();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        QuotePartHub quotePartHub = quoteHub.ToQuotePartHub(CandlePart.OHL3);
        SmaHub smaHub = quoteHub.ToSmaHub(20);
        TrHub trHub = quoteHub.ToTrHub();

        // emulate adding quotes to hub
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Add(quotes[80]);

        // end all observations
        quoteHub.EndTransmission();

        // get static equivalents for comparison
        IReadOnlyList<AdlResult> staticAdl = quotes.ToAdl();
        IReadOnlyList<AtrResult> staticAtr = quotes.ToAtr();
        IReadOnlyList<AtrStopResult> staticAtrStop = quotes.ToAtrStop();
        IReadOnlyList<AlligatorResult> staticAlligator = quotes.ToAlligator();
        IReadOnlyList<EmaResult> staticEma = quotes.ToEma(20);
        IReadOnlyList<TimeValue> staticQuotePart = quotes.Use(CandlePart.OHL3);
        IReadOnlyList<SmaResult> staticSma = quotes.ToSma(20);
        IReadOnlyList<TrResult> staticTr = quotes.ToTr();

        // final results should persist in scope
        IReadOnlyList<AdlResult> streamAdl = adlHub.Results;
        IReadOnlyList<AtrResult> streamAtr = atrHub.Results;
        IReadOnlyList<AtrStopResult> streamAtrStop = atrStopHub.Results;
        IReadOnlyList<AlligatorResult> streamAlligator = alligatorHub.Results;
        IReadOnlyList<EmaResult> streamEma = emaHub.Results;
        IReadOnlyList<TimeValue> streamQuotePart = quotePartHub.Results;
        IReadOnlyList<SmaResult> streamSma = smaHub.Results;
        IReadOnlyList<TrResult> streamTr = trHub.Results;

        // assert, should be correct length
        length.Should().Be(502);
        streamAdl.Should().HaveCount(length);
        streamAlligator.Should().HaveCount(length);
        streamAtr.Should().HaveCount(length);
        streamAtrStop.Should().HaveCount(length);
        streamEma.Should().HaveCount(length);
        streamQuotePart.Should().HaveCount(length);
        streamSma.Should().HaveCount(length);
        streamTr.Should().HaveCount(length);

        // assert, should equal static series
        streamAdl.IsExactly(staticAdl);
        streamAtr.IsExactly(staticAtr);
        streamAtrStop.IsExactly(staticAtrStop);
        streamAlligator.IsExactly(staticAlligator);
        streamEma.IsExactly(staticEma);
        streamQuotePart.IsExactly(staticQuotePart);
        streamSma.IsExactly(staticSma);
        streamTr.IsExactly(staticTr);
    }
}
