// quote list converters

namespace Utilities;

[TestClass]
public partial class Quotes : TestBase
{
    [TestMethod]
    public void ToSortedList()
    {
        IReadOnlyList<Quote> quotes = Data.GetMismatch();

        IReadOnlyList<Quote> h = quotes.ToSortedList();

        // proper quantities
        h.Should().HaveCount(502);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", invariantCulture);
        h[0].Timestamp.Should().Be(firstDate);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture);
        h[^1].Timestamp.Should().Be(lastDate);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", invariantCulture);
        h[50].Timestamp.Should().Be(spotDate);
    }

    [TestMethod]
    public void ToQuoteList()
    {
        // setup
        IReadOnlyList<Quote> quotes
            = Quotes.Take(5).ToList();

        IReadOnlyList<MyQuote> myQuotes = quotes
            .Select(static x => new MyQuote {
                Timestamp = x.Timestamp,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                Close = x.Close,
                Volume = x.Volume
            }).ToList();

        // sut
        IReadOnlyList<Quote> sut
            = myQuotes.ToQuoteList();

        // assert is same as original
        sut.IsExactly(quotes);
    }

    [TestMethod]
    public void ToQuote()
    {
        // setup
        Quote q = Quotes[0];

        MyQuote myQuote = new() {
            Timestamp = q.Timestamp,
            Open = q.Open,
            High = q.High,
            Low = q.Low,
            Close = q.Close,
            Volume = q.Volume
        };

        // sut
        Quote sut = myQuote.ToQuote();

        // assert value based equality
        sut.Should().Be(q);
        sut.Value.Should().Be(q.Value);
    }

    private class MyQuote : IQuote
    {
        public DateTime Timestamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }

        public double Value => (double)Close;
    }
}
