namespace Catalogging;

/// <summary>
/// Test class for QuotePart catalog functionality.
/// </summary>
[TestClass]
public class QuotePartTests : TestBase
{
    [TestMethod]
    public void QuotePartSeriesListing()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = QuoteParts.SeriesListing;
        listing.Should().NotBeNull("QuotePart Series listing should be found");

        // Get catalog default value for test use
        IndicatorParam candlePartParam = listing.Parameters.Single(static p => p.ParameterName == "candlePart");
        CandlePart candlePartValue = (CandlePart)candlePartParam.DefaultValue;

        // Act - Use catalog utility to dynamically execute the indicator
        IReadOnlyList<QuotePart> catalogResults = ListingExecutor.Execute<QuotePart>(quotes, listing);

        // Act - Direct call for comparison using catalog's default parameter value
        IReadOnlyList<QuotePart> directResults = quotes.ToQuotePart(candlePartValue);

        // Assert - Results from catalog-driven execution should match direct call
        catalogResults.IsExactly(directResults);

        // Assert - Lookup gets correct values
        candlePartValue.Should().Be(CandlePart.Close);

        // Additional assertions on catalog metadata (basic validation)
        listing.Name.Should().Be("Quote Part");
        listing.Uiid.Should().Be("QUOTEPART");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceTransform);
        listing.MethodName.Should().Be("ToQuotePart");
        listing.Parameters.Should().HaveCount(1);
        listing.Results.Should().HaveCount(1);
    }

    [TestMethod]
    public void QuotePartStreamListing()
    {
        // Act
        IndicatorListing listing = QuoteParts.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Quote Part");
        listing.Uiid.Should().Be("QUOTEPART");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.PriceTransform);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam candlePart = listing.Parameters.FirstOrDefault(static p => p.ParameterName == "candlePart");
        candlePart.Should().NotBeNull();
        candlePart?.DisplayName.Should().Be("Candle Part");

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult result = listing.Results[0];
        result.DataName.Should().Be("Value");
        result.DisplayName.Should().Be("Value");
        result.DataType.Should().Be(ResultType.Default);
        result.IsReusable.Should().BeTrue();
    }

    [TestMethod]
    public void QuotePartBufferListing()
    {
        // Act
        IndicatorListing listing = QuoteParts.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Quote Part");
        listing.Uiid.Should().Be("QUOTEPART");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.PriceTransform);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam candlePart = listing.Parameters.FirstOrDefault(static p => p.ParameterName == "candlePart");
        candlePart.Should().NotBeNull();
        candlePart?.DisplayName.Should().Be("Candle Part");

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult result = listing.Results[0];
        result.DataName.Should().Be("Value");
        result.DisplayName.Should().Be("Value");
        result.DataType.Should().Be(ResultType.Default);
        result.IsReusable.Should().BeTrue();
    }

    [TestMethod]
    public void CatalogBrowsingIncludesQuotePartSeries()
    {
        // Act
        IReadOnlyCollection<IndicatorListing> catalog = Catalog.Get();
        IndicatorListing quotePartSeries = Catalog.Get("QUOTEPART", Style.Series);
        IReadOnlyCollection<IndicatorListing> seriesListings = Catalog.Get(Style.Series);

        // Assert
        catalog.Should().NotBeNull();
        catalog.Should().Contain(static l => l.Uiid == "QUOTEPART" && l.Style == Style.Series);
        quotePartSeries.Should().NotBeNull();
        seriesListings.Should().Contain(static l => l.Uiid == "QUOTEPART" && l.Style == Style.Series);
    }
}
