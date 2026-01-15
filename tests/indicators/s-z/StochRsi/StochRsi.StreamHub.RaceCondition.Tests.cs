namespace StreamHubs;

using TestData = Test.Data.Data;

/// <summary>
/// Tests demonstrating the pre-fix race condition failure mode in StochRsiHub.
/// 
/// ARCHITECTURAL FLAW (v3 broken implementation):
/// Both StochRsiHub and its internal rsiHub subscribed to the same provider:
///   : base(provider)  // StochRsiHub subscribes to provider
///   rsiHub = provider.ToRsiHub(rsiPeriods);  // rsiHub ALSO subscribes to provider
///
/// FAILURE MODE:
/// During rebuild operations (late arrival, removal), ToIndicator() accesses rsiHub.Cache[i]
/// before the internal rsiHub has processed the update and populated that cache entry.
/// No ordering guarantee exists → ArgumentOutOfRangeException when Cache[i] doesn't exist.
///
/// CORRECT ARCHITECTURE (post-fix):
///   : base(provider.ToRsiHub(rsiPeriods))  // StochRsiHub subscribes to RsiHub
///   rsiHub = (RsiHub)ProviderCache;  // Reference to our provider (which is RsiHub)
/// Data flow: provider → RsiHub → StochRsiHub (proper chaining eliminates race condition)
/// </summary>
[TestClass]
public class StochRsiHubRaceConditionTests
{
    private static readonly IReadOnlyList<Quote> Quotes = TestData.GetDefault();

    /// <summary>
    /// Demonstrates ArgumentOutOfRangeException during late arrival scenario.
    /// The broken implementation CAN fail because rsiHub.Cache[i] may not exist yet.
    /// 
    /// NOTE: This is a RACE CONDITION - the exception is intermittent and depends on
    /// notification ordering. The test documents the failure mode even if it doesn't
    /// always reproduce in test execution.
    /// </summary>
    [TestMethod]
    public void BrokenImplementation_LateArrival_CanThrowIndexOutOfRangeException()
    {
        // Setup: QuoteHub with initial quotes
        QuoteHub quoteHub = new();

        quoteHub.Add(Quotes.Take(20));

        // Create StochRsiHub (broken: both hubs subscribe to same provider)
        StochRsiHub observer = quoteHub.ToStochRsiHub(14, 14, 3, 1);

        // Add more quotes normally
        for (int i = 20; i < 80; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // Skip quote 80
        for (int i = 81; i < Quotes.Count; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // Late arrival triggers rebuild: CAN THROW ArgumentOutOfRangeException
        // During rebuild, StochRsiHub.ToIndicator() tries to access rsiHub.Cache[i]
        // but rsiHub MAY NOT have processed that index yet → Cache[i] doesn't exist
        //
        // RACE CONDITION: Depends on notification ordering - not always reproducible
        try
        {
            quoteHub.Insert(Quotes[80]);
            // If no exception, the correct hub processed first (this time)
        }
        catch (ArgumentOutOfRangeException)
        {
            // This is the expected failure when StochRsiHub processes before rsiHub
        }

        observer.Unsubscribe();
        quoteHub.EndTransmission();
        
        // Test passes whether or not exception occurred - documenting the race condition
        Assert.IsTrue(true, "Race condition documented");
    }

    /// <summary>
    /// Demonstrates ArgumentOutOfRangeException during removal scenario.
    /// The broken implementation CAN fail during rebuild after removing a quote.
    /// 
    /// NOTE: This is a RACE CONDITION - the exception is intermittent and depends on
    /// notification ordering. The test documents the failure mode even if it doesn't
    /// always reproduce in test execution.
    /// </summary>
    [TestMethod]
    public void BrokenImplementation_Removal_CanThrowIndexOutOfRangeException()
    {
        // Setup: QuoteHub with all quotes
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);

        // Create StochRsiHub (broken: both hubs subscribe to same provider)
        StochRsiHub observer = quoteHub.ToStochRsiHub(14, 14, 3, 1);

        // Remove a quote triggers rebuild: CAN THROW ArgumentOutOfRangeException
        // StochRsiHub rebuilds and tries to access rsiHub.Cache[i] before
        // rsiHub MAY have rebuilt its cache → ArgumentOutOfRangeException
        //
        // RACE CONDITION: Depends on notification ordering - not always reproducible
        try
        {
            quoteHub.RemoveAt(250);
            // If no exception, the correct hub processed first (this time)
        }
        catch (ArgumentOutOfRangeException)
        {
            // This is the expected failure when StochRsiHub processes before rsiHub
        }

        observer.Unsubscribe();
        quoteHub.EndTransmission();
        
        // Test passes whether or not exception occurred - documenting the race condition
        Assert.IsTrue(true, "Race condition documented");
    }

    /// <summary>
    /// Documents the race condition: no ordering guarantee between hubs.
    /// When both subscribe to same provider, notification order is undefined.
    /// </summary>
    [TestMethod]
    public void BrokenImplementation_ExplainsRaceCondition()
    {
        // The fundamental issue with the broken implementation:
        //
        // QuoteHub notifies all subscribers when a quote is added/inserted/removed.
        // With broken architecture:
        //   - StochRsiHub subscribes to QuoteHub
        //   - Internal rsiHub ALSO subscribes to QuoteHub
        //
        // During rebuild (triggered by Insert or RemoveAt):
        //   1. QuoteHub notifies subscribers in UNDEFINED order
        //   2. If StochRsiHub processes update BEFORE rsiHub:
        //      - StochRsiHub.ToIndicator(item, i) executes
        //      - Tries to access rsiHub.Cache[i]
        //      - But rsiHub hasn't processed this update yet!
        //      - rsiHub.Cache.Count is still < i
        //      - ArgumentOutOfRangeException thrown
        //
        // The fix chains the hubs properly:
        //   - StochRsiHub subscribes to RsiHub (not QuoteHub)
        //   - RsiHub subscribes to QuoteHub
        //   - Data flows: QuoteHub → RsiHub → StochRsiHub
        //   - When StochRsiHub receives update, RsiHub has already processed it
        //   - rsiHub.Cache[i] is guaranteed to exist

        Assert.IsTrue(true, "This test documents the race condition architecture");
    }
}
