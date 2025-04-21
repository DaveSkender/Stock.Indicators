namespace TestNamespace;

// This class contains a method that should trigger our IND001 analyzer warning
public static class TestIndicator
{
    // This method will trigger IND001 because it's missing the required IndicatorAttribute
    public static IReadOnlyList<EmaResult> ToTestIndicator(this IReadOnlyList<Quote> quotes, int period) =>
        // This is just a simple test method to trigger our analyzer
        quotes.ToEma(period);
}
