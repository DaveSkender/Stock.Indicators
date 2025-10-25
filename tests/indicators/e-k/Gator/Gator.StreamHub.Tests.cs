namespace StreamHub;

[TestClass]
public class GatorHubTests : StreamHubTestBase, ITestChainObserver
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        GatorHub observer = quoteHub
            .ToGatorHub();

        // fetch initial results (early)
        IReadOnlyList<GatorResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotesList[80]);

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<GatorResult> seriesList
           = quotesList
            .ToGator();

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        GatorHub observer = quoteHub
            .ToSmaHub(10)
            .ToGatorHub();

        // emulate adding quotes out of order
        // note: this works when graceful order
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotesList[80]);

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<GatorResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<GatorResult> seriesList
           = quotesList
            .ToSma(10)
            .ToGator();

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Reset()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        GatorHub observer = quoteHub.ToGatorHub();

        // Add ~50 quotes to populate state
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // assert observer.Results has 50 entries and the last result has non-null values
        observer.Results.Should().HaveCount(50);
        observer.Results[^1].Upper.Should().NotBeNull();
        observer.Results[^1].Lower.Should().NotBeNull();

        // call observer.Reinitialize() - this resets the subscription and rebuilds from quoteHub
        observer.Reinitialize();

        // The observer should still have 50 results since it rebuilds from the quoteHub
        observer.Results.Should().HaveCount(50);
        observer.Results[^1].Upper.Should().NotBeNull();
        observer.Results[^1].Lower.Should().NotBeNull();

        // Now test with a completely fresh setup after unsubscribing
        observer.Unsubscribe();
        quoteHub.EndTransmission();

        // Create a new quoteHub with just one quote
        QuoteHub freshProvider = new();
        GatorHub freshObserver = freshProvider.ToGatorHub();

        // Add one quote and assert observer.Results has count 1 and that values are null (warmup period)
        freshProvider.Add(quotesList[0]);

        freshObserver.Results.Should().HaveCount(1);
        freshObserver.Results[^1].Upper.Should().BeNull();
        freshObserver.Results[^1].Lower.Should().BeNull();

        // cleanup
        freshObserver.Unsubscribe();
        freshProvider.EndTransmission();
    }

    [TestMethod]
    public void GatorHubExceptions()
    {
        QuoteHub quoteHub = new();
        AlligatorHub alligatorHub = quoteHub.ToAlligatorHub();

        // test null alligatorHub parameter (throws NullReferenceException from base constructor)
        Action act1 = () => _ = new GatorHub(null!);
        act1.Should().Throw<NullReferenceException>("AlligatorHub cannot be null");

        // test null item in ToIndicator (via reflection to access protected method)
        GatorHub gatorHub = alligatorHub.ToGatorHub();
        Action act2 = () => {
            System.Reflection.MethodInfo method = typeof(GatorHub).GetMethod(
                "ToIndicator",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            method?.Invoke(gatorHub, [null, null]);
        };
        act2.Should().Throw<System.Reflection.TargetInvocationException>()
            .WithInnerException<ArgumentNullException>("item cannot be null");

        gatorHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        GatorHub hub = new(new AlligatorHub(new QuoteHub(), 13, 8, 8, 5, 5, 3));
        hub.ToString().Should().Be("GATOR()");
    }
}
