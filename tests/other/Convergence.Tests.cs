namespace Behavioral;

[TestClass]
public class Convergence : TestBase
{
    private static readonly int[] QuotesQuantities =
        [5, 14, 28, 40, 50, 75, 100, 110, 120, 130, 140, 150, 160, 175, 200, 250, 350, 500, 600, 700, 800, 900, 1000];

    [TestMethod]
    public void Adx()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<AdxResult> r = qts.GetAdx();

            AdxResult l = r.LastOrDefault();
            Console.WriteLine($"ADX(14) on {l.Timestamp:d} with {qts.Count(),4} historical qts: {l.Adx:N8}");
        }
    }

    [TestMethod]
    public void Alligator()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = Data.GetLongish(qty);
            IEnumerable<AlligatorResult> r = quotes.GetAlligator();

            AlligatorResult l = r.LastOrDefault();
            Console.WriteLine(
                "ALLIGATOR(13,8,5) on {0:d} with {1,4} periods: Jaw {2:N8}",
                l.Timestamp, quotes.Count(), l.Jaw);
        }
    }

    [TestMethod]
    public void Atr()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<AtrResult> r = qts.GetAtr();

            AtrResult l = r.LastOrDefault();
            Console.WriteLine($"ATR(14) on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Atr:N8}");
        }
    }

    [TestMethod]
    public void ChaikinOsc()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<ChaikinOscResult> r = qts.GetChaikinOsc();

            ChaikinOscResult l = r.LastOrDefault();
            Console.WriteLine($"CHAIKIN OSC on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Oscillator:N8}");
        }
    }

    [TestMethod]
    public void ConnorsRsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<ConnorsRsiResult> r = qts.GetConnorsRsi(3, 2, 10);

            ConnorsRsiResult l = r.LastOrDefault();
            Console.WriteLine($"CRSI on {l.Timestamp:d} with {qts.Count(),4} periods: {l.ConnorsRsi:N8}");
        }
    }

    [TestMethod]
    public void Dema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<DemaResult> r = qts.GetDema(15);

            DemaResult l = r.LastOrDefault();
            Console.WriteLine($"DEMA(15) on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Dema:N8}");
        }
    }

    [TestMethod]
    public void Dynamic()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<DynamicResult> r = qts.GetDynamic(100);

            DynamicResult l = r.LastOrDefault();
            Console.WriteLine($"DYNAMIC(15) on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Dynamic:N8}");
        }
    }

    [TestMethod]
    public void Ema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<EmaResult> r = qts.ToEma(15);

            EmaResult l = r.LastOrDefault();
            Console.WriteLine($"EMA(15) on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Ema:N8}");
        }
    }

    [TestMethod]
    public void FisherTransform()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<FisherTransformResult> r = qts.GetFisherTransform();

            FisherTransformResult l = r.LastOrDefault();
            Console.WriteLine($"FT(10) on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Fisher:N8}");
        }
    }

    [TestMethod]
    public void Gator()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = Data.GetLongish(qty);
            IEnumerable<GatorResult> r = quotes.GetGator();

            GatorResult l = r.LastOrDefault();
            Console.WriteLine(
                "GATOR() on {0:d} with {1,4} periods: Upper {2:N8}  Lower {3:N8}",
                l.Timestamp, quotes.Count(), l.Upper, l.Lower);
        }
    }

    [TestMethod]
    public void HtTrendline()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<HtlResult> r = qts.GetHtTrendline();

            HtlResult l = r.LastOrDefault();
            Console.WriteLine($"HTL on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Trendline:N8}");
        }
    }

    [TestMethod]
    public void Kama()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<KamaResult> r = qts.GetKama();

            KamaResult l = r.LastOrDefault();
            Console.WriteLine($"KAMA(10) on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Kama:N8}");
        }
    }

    [TestMethod]
    public void Keltner()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<KeltnerResult> r = qts.GetKeltner(100);

            KeltnerResult l = r.LastOrDefault();
            Console.WriteLine($"KC-UP on {l.Timestamp:d} with {qts.Count(),4} periods: {l.UpperBand:N8}");
        }
    }

    [TestMethod]
    public void Macd()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(15 + qty);
            IEnumerable<MacdResult> r = qts.GetMacd();

            MacdResult l = r.LastOrDefault();
            Console.WriteLine($"MACD on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Macd:N8}");
        }
    }

    [TestMethod]
    public void Mama()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<MamaResult> r = qts.GetMama();

            MamaResult l = r.LastOrDefault();
            Console.WriteLine($"MAMA on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Mama:N8}");
        }
    }

    [TestMethod]
    public void Pmo()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<PmoResult> r = qts.GetPmo();

            PmoResult l = r.LastOrDefault();
            Console.WriteLine($"PMO on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Pmo:N8}");
        }
    }

    [TestMethod]
    public void Pvo()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<PvoResult> r = qts.GetPvo();

            PvoResult l = r.LastOrDefault();
            Console.WriteLine($"PVO on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Pvo:N8}");
        }
    }

    [TestMethod]
    public void Rsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<RsiResult> r = qts.GetRsi();

            RsiResult l = r.LastOrDefault();
            Console.WriteLine($"RSI(14) on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Rsi:N8}");
        }
    }

    [TestMethod]
    public void Smi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetDefault(qty);
            IEnumerable<SmiResult> r = qts.GetSmi(14, 20, 5);

            SmiResult l = r.LastOrDefault();
            Console.WriteLine($"SMI on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Smi:N8}");
        }
    }

    [TestMethod]
    public void Smma()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<SmmaResult> r = qts.GetSmma(15);

            SmmaResult l = r.LastOrDefault();
            Console.WriteLine($"SMMA(15) on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Smma:N8}");
        }
    }

    [TestMethod]
    public void StarcBands()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<StarcBandsResult> r = qts.GetStarcBands(20);

            StarcBandsResult l = r.LastOrDefault();
            Console.WriteLine($"STARC UPPER on {l.Timestamp:d} with {qts.Count(),4} periods: {l.UpperBand:N8}");
        }
    }

    [TestMethod]
    public void StochRsi()
    {
        foreach (int qty in QuotesQuantities.Where(x => x <= 502))
        {
            IEnumerable<Quote> qts = Data.GetDefault(qty);
            IEnumerable<StochRsiResult> r = qts.GetStochRsi(14, 14, 3);

            StochRsiResult l = r.LastOrDefault();
            Console.WriteLine($"SRSI on {l.Timestamp:d} with {qts.Count(),4} periods: {l.StochRsi:N8}");
        }
    }

    [TestMethod]
    public void T3()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<T3Result> r = qts.GetT3(20);

            T3Result l = r.LastOrDefault();
            Console.WriteLine($"T3 on {l.Timestamp:d} with {qts.Count(),4} periods: {l.T3:N8}");
        }
    }

    [TestMethod]
    public void Tema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<TemaResult> r = qts.GetTema(15);

            TemaResult l = r.LastOrDefault();
            Console.WriteLine($"TEMA on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Tema:N8}");
        }
    }

    [TestMethod]
    public void Trix()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<TrixResult> r = qts.GetTrix(15);

            TrixResult l = r.LastOrDefault();
            Console.WriteLine($"TRIX on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Trix:N8}");
        }
    }

    [TestMethod]
    public void Tsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(20 + qty);
            IEnumerable<TsiResult> r = qts.GetTsi();

            TsiResult l = r.LastOrDefault();
            Console.WriteLine($"TSI on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Tsi:N8}");
        }
    }

    [TestMethod]
    public void Vortex()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> qts = Data.GetLongish(qty);
            IEnumerable<VortexResult> r = qts.GetVortex(14);

            VortexResult l = r.LastOrDefault();
            Console.WriteLine($"VI+ on {l.Timestamp:d} with {qts.Count(),4} periods: {l.Pvi:N8}");
        }
    }
}
