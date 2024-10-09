[assembly: CLSCompliant(true)]
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace PublicApi;

// PUBLIC API (INTERFACES)

[TestClass]
public class UserInterface
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();

    [TestMethod]
    public void QuoteValidation()
    {
        IEnumerable<Quote> enumerable = quotes;

        enumerable.Validate();
        enumerable.GetSma(6);
        enumerable.ToEma(5);
    }

    [TestMethod]
    public void QuoteValidationReturn()
    {
        IReadOnlyList<Quote> h = quotes.Validate();

        Quote f = h[0];
        Console.WriteLine($"Quote:{f}");
    }

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

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observers
        AdlHub<Quote> adlHub = provider.ToAdl();
        AlligatorHub<Quote> alligatorHub = provider.ToAlligator();
        AtrHub<Quote> atrHub = provider.ToAtr();
        AtrStopHub<Quote> atrStopHub = provider.ToAtrStop();
        EmaHub<Quote> emaHub = provider.ToEma(20);
        QuotePartHub<Quote> quotePartHub = provider.ToQuotePart(CandlePart.OHL3);
        SmaHub<Quote> smaHub = provider.ToSma(20);
        TrHub<Quote> trHub = provider.ToTr();

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotes[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Insert(quotes[80]);

        // end all observations
        provider.EndTransmission();

        // get static equivalents for comparison
        IEnumerable<AdlResult> staticAdl = quotes.GetAdl();
        IReadOnlyList<AtrResult> staticAtr = quotes.GetAtr();
        IReadOnlyList<AtrStopResult> staticAtrStop = quotes.GetAtrStop();
        IEnumerable<AlligatorResult> staticAlligator = quotes.GetAlligator();
        IEnumerable<EmaResult> staticEma = quotes.ToEma(20);
        IEnumerable<QuotePart> staticQuotePart = quotes.Use(CandlePart.OHL3);
        IEnumerable<SmaResult> staticSma = quotes.GetSma(20);
        IReadOnlyList<TrResult> staticTr = quotes.GetTr();

        // final results should persist in scope
        IReadOnlyList<AdlResult> streamAdl = adlHub.Results;
        IReadOnlyList<AtrResult> streamAtr = atrHub.Results;
        IReadOnlyList<AtrStopResult> streamAtrStop = atrStopHub.Results;
        IReadOnlyList<AlligatorResult> streamAlligator = alligatorHub.Results;
        IReadOnlyList<EmaResult> streamEma = emaHub.Results;
        IReadOnlyList<QuotePart> streamQuotePart = quotePartHub.Results;
        IReadOnlyList<SmaResult> streamSma = smaHub.Results;
        IReadOnlyList<TrResult> streamTr = trHub.Results;

        // assert, should be correct length
        streamAdl.Should().HaveCount(length);
        streamAlligator.Should().HaveCount(length);
        streamAtr.Should().HaveCount(length);
        streamAtrStop.Should().HaveCount(length);
        streamEma.Should().HaveCount(length);
        streamQuotePart.Should().HaveCount(length);
        streamSma.Should().HaveCount(length);
        streamTr.Should().HaveCount(length);

        // assert, should equal static series
        streamAdl.Should().BeEquivalentTo(staticAdl);
        streamAtr.Should().BeEquivalentTo(staticAtr);
        streamAtrStop.Should().BeEquivalentTo(staticAtrStop);
        streamAlligator.Should().BeEquivalentTo(staticAlligator);
        streamEma.Should().BeEquivalentTo(staticEma);
        streamQuotePart.Should().BeEquivalentTo(staticQuotePart);
        streamSma.Should().BeEquivalentTo(staticSma);
        streamTr.Should().BeEquivalentTo(staticTr);
    }
}
