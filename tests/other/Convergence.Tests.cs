namespace Tests.Other;

[TestClass]
public class ConvergenceTests : TestBase
{
    private static readonly int[] QuotesQuantities =
        [5, 14, 28, 40, 50, 75, 100, 110, 120, 130, 140, 150, 160, 175, 200, 250, 350, 500, 600, 700, 800, 900, 1000];

    [TestMethod]
    public void Adx()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<AdxResult> r = quotes.GetAdx(14);

            AdxResult l = r.LastOrDefault();
            Console.WriteLine($"ADX(14) on {l.Date:d} with {quotes.Count(),4} historical quotes: {l.Adx:N8}");
        }
    }

    [TestMethod]
    public void Atr()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<AtrResult> r = quotes.GetAtr(14);

            AtrResult l = r.LastOrDefault();
            Console.WriteLine($"ATR(14) on {l.Date:d} with {quotes.Count(),4} periods: {l.Atr:N8}");
        }
    }

    [TestMethod]
    public void ChaikinOsc()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<ChaikinOscResult> r = quotes.GetChaikinOsc();

            ChaikinOscResult l = r.LastOrDefault();
            Console.WriteLine($"CHAIKIN OSC on {l.Date:d} with {quotes.Count(),4} periods: {l.Oscillator:N8}");
        }
    }

    [TestMethod]
    public void ConnorsRsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<ConnorsRsiResult> r = quotes.GetConnorsRsi(3, 2, 10);

            ConnorsRsiResult l = r.LastOrDefault();
            Console.WriteLine($"CRSI on {l.Date:d} with {quotes.Count(),4} periods: {l.ConnorsRsi:N8}");
        }
    }

    [TestMethod]
    public void Dema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<DemaResult> r = quotes.GetDema(15);

            DemaResult l = r.LastOrDefault();
            Console.WriteLine($"DEMA(15) on {l.Date:d} with {quotes.Count(),4} periods: {l.Dema:N8}");
        }
    }

    [TestMethod]
    public void Dynamic()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<DynamicResult> r = quotes.GetDynamic(100);

            DynamicResult l = r.LastOrDefault();
            Console.WriteLine($"DYNAMIC(15) on {l.Date:d} with {quotes.Count(),4} periods: {l.Dynamic:N8}");
        }
    }

    [TestMethod]
    public void Ema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<EmaResult> r = quotes.GetEma(15);

            EmaResult l = r.LastOrDefault();
            Console.WriteLine($"EMA(15) on {l.Date:d} with {quotes.Count(),4} periods: {l.Ema:N8}");
        }
    }

    [TestMethod]
    public void FisherTransform()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<FisherTransformResult> r = quotes.GetFisherTransform(10);

            FisherTransformResult l = r.LastOrDefault();
            Console.WriteLine($"FT(10) on {l.Date:d} with {quotes.Count(),4} periods: {l.Fisher:N8}");
        }
    }

    [TestMethod]
    public void HtTrendline()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<HtlResult> r = quotes.GetHtTrendline();

            HtlResult l = r.LastOrDefault();
            Console.WriteLine($"HTL on {l.Date:d} with {quotes.Count(),4} periods: {l.Trendline:N8}");
        }
    }

    [TestMethod]
    public void Kama()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<KamaResult> r = quotes.GetKama(10);

            KamaResult l = r.LastOrDefault();
            Console.WriteLine($"KAMA(10) on {l.Date:d} with {quotes.Count(),4} periods: {l.Kama:N8}");
        }
    }

    [TestMethod]
    public void Keltner()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<KeltnerResult> r = quotes.GetKeltner(100);

            KeltnerResult l = r.LastOrDefault();
            Console.WriteLine($"KC-UP on {l.Date:d} with {quotes.Count(),4} periods: {l.UpperBand:N8}");
        }
    }

    [TestMethod]
    public void Macd()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(15 + qty);
            IEnumerable<MacdResult> r = quotes.GetMacd();

            MacdResult l = r.LastOrDefault();
            Console.WriteLine($"MACD on {l.Date:d} with {quotes.Count(),4} periods: {l.Macd:N8}");
        }
    }

    [TestMethod]
    public void Mama()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<MamaResult> r = quotes.GetMama();

            MamaResult l = r.LastOrDefault();
            Console.WriteLine($"MAMA on {l.Date:d} with {quotes.Count(),4} periods: {l.Mama:N8}");
        }
    }

    [TestMethod]
    public void Pmo()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<PmoResult> r = quotes.GetPmo();

            PmoResult l = r.LastOrDefault();
            Console.WriteLine($"PMO on {l.Date:d} with {quotes.Count(),4} periods: {l.Pmo:N8}");
        }
    }

    [TestMethod]
    public void Pvo()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<PvoResult> r = quotes.GetPvo();

            PvoResult l = r.LastOrDefault();
            Console.WriteLine($"PVO on {l.Date:d} with {quotes.Count(),4} periods: {l.Pvo:N8}");
        }
    }

    [TestMethod]
    public void Rsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<RsiResult> r = quotes.GetRsi(14);

            RsiResult l = r.LastOrDefault();
            Console.WriteLine($"RSI(14) on {l.Date:d} with {quotes.Count(),4} periods: {l.Rsi:N8}");
        }
    }

    [TestMethod]
    public void Smi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetDefault(qty);
            IEnumerable<SmiResult> r = quotes.GetSmi(14, 20, 5, 3);

            SmiResult l = r.LastOrDefault();
            Console.WriteLine($"SMI on {l.Date:d} with {quotes.Count(),4} periods: {l.Smi:N8}");
        }
    }

    [TestMethod]
    public void Smma()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<SmmaResult> r = quotes.GetSmma(15);

            SmmaResult l = r.LastOrDefault();
            Console.WriteLine($"SMMA(15) on {l.Date:d} with {quotes.Count(),4} periods: {l.Smma:N8}");
        }
    }

    [TestMethod]
    public void StarcBands()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<StarcBandsResult> r = quotes.GetStarcBands(20);

            StarcBandsResult l = r.LastOrDefault();
            Console.WriteLine($"STARC UPPER on {l.Date:d} with {quotes.Count(),4} periods: {l.UpperBand:N8}");
        }
    }

    [TestMethod]
    public void StochRsi()
    {
        foreach (int qty in QuotesQuantities.Where(x => x <= 502))
        {
            IEnumerable<Quote> quotes = TestData.GetDefault(qty);
            IEnumerable<StochRsiResult> r = quotes.GetStochRsi(14, 14, 3, 1);

            StochRsiResult l = r.LastOrDefault();
            Console.WriteLine($"SRSI on {l.Date:d} with {quotes.Count(),4} periods: {l.StochRsi:N8}");
        }
    }

    [TestMethod]
    public void T3()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<T3Result> r = quotes.GetT3(20);

            T3Result l = r.LastOrDefault();
            Console.WriteLine($"T3 on {l.Date:d} with {quotes.Count(),4} periods: {l.T3:N8}");
        }
    }

    [TestMethod]
    public void Tema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<TemaResult> r = quotes.GetTema(15);

            TemaResult l = r.LastOrDefault();
            Console.WriteLine($"TEMA on {l.Date:d} with {quotes.Count(),4} periods: {l.Tema:N8}");
        }
    }

    [TestMethod]
    public void Trix()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<TrixResult> r = quotes.GetTrix(15);

            TrixResult l = r.LastOrDefault();
            Console.WriteLine($"TRIX on {l.Date:d} with {quotes.Count(),4} periods: {l.Trix:N8}");
        }
    }

    [TestMethod]
    public void Tsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(20 + qty);
            IEnumerable<TsiResult> r = quotes.GetTsi();

            TsiResult l = r.LastOrDefault();
            Console.WriteLine($"TSI on {l.Date:d} with {quotes.Count(),4} periods: {l.Tsi:N8}");
        }
    }

    [TestMethod]
    public void Vortex()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<VortexResult> r = quotes.GetVortex(14);

            VortexResult l = r.LastOrDefault();
            Console.WriteLine($"VI+ on {l.Date:d} with {quotes.Count(),4} periods: {l.Pvi:N8}");
        }
    }
}
