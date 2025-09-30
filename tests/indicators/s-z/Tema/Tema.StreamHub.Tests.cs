namespace StreamHub;

[TestClass]
public class TemaHub : StreamHubTestBase, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        int lookbackPeriods = 20;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        TemaHub<Quote> observer = provider
            .ToTema(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<TemaResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Insert(quotesList[80]);

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<TemaResult> seriesList = quotesList
            .ToTema(lookbackPeriods);

        // assert, should equal series
        streamList.Should().BeEquivalentTo(seriesList);
        streamList.Should().HaveCount(501);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int temaPeriods = 20;
        int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        SmaHub<TemaResult> observer = provider
            .ToTema(temaPeriods)
            .ToSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToTema(temaPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().BeEquivalentTo(seriesList);
        streamList.Should().HaveCount(501);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        TemaHub<Quote> hub = new(new QuoteHub<Quote>(), 20);
        hub.ToString().Should().Be("TEMA(20)");
    }

    [TestMethod]
    public void SettingsInheritance()
    {
        // setup quote hub (1st level)
        QuoteHub<Quote> quoteHub = new();

        // setup tema hub (2nd level)
        TemaHub<Quote> temaHub = quoteHub
            .ToTema(lookbackPeriods: 20);

        // setup child hub (3rd level)
        SmaHub<TemaResult> childHub = temaHub
            .ToSma(lookbackPeriods: 5);

        // note: despite `quoteHub` being parentless,
        // it has default properties; it should not
        // inherit its own empty provider settings

        // assert
        quoteHub.Properties.Settings.Should().Be(0b00000000, "it has default settings, not inherited");
        temaHub.Properties.Settings.Should().Be(0b00000000, "it has default properties");
        childHub.Properties.Settings.Should().Be(0b00000000, "it inherits default properties");
    }
}
