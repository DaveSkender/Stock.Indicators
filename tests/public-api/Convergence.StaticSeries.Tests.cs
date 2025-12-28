#nullable enable

namespace Behavioral;

[TestClass, TestCategory("Integration")]
public class ConvergenceSeries : TestBase
{
    private static readonly int[] QuotesQuantities =
        [5, 14, 28, 40, 50, 75, 100, 110, 120, 130, 140, 150, 160, 175, 200, 250, 350, 500, 600, 700, 800, 900, 1000];

    private static IReadOnlyList<Quote> Qts => Data.GetLongish();

    [TestMethod]
    public void Adx()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<AdxResult> r = Qts.ToAdx();

            AdxResult l = r[^1];
            Console.WriteLine($"ADX(14) on {l.Timestamp:d} with {Qts.Count,4} historical qts: {l.Adx:N8}");
        }
    }

    [TestMethod]
    public void Alligator()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<AlligatorResult> r = Qts.ToAlligator();

            AlligatorResult l = r[^1];
            Console.WriteLine(
                "ALLIGATOR(13,8,5) on {0:d} with {1,4} periods: Jaw {2:N8}",
                l.Timestamp, Qts.Count, l.Jaw);
        }
    }

    [TestMethod]
    public void Atr()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<AtrResult> r = Qts.ToAtr();

            AtrResult l = r[^1];
            Console.WriteLine($"ATR(14) on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Atr:N8}");
        }
    }

    [TestMethod]
    public void ChaikinOsc()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<ChaikinOscResult> r = Qts.ToChaikinOsc();

            ChaikinOscResult l = r[^1];
            Console.WriteLine($"CHAIKIN OSC on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Oscillator:N8}");
        }
    }

    [TestMethod]
    public void ConnorsRsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<ConnorsRsiResult> r = Qts.ToConnorsRsi(3, 2, 10);

            ConnorsRsiResult l = r[^1];
            Console.WriteLine($"CRSI on {l.Timestamp:d} with {Qts.Count,4} periods: {l.ConnorsRsi:N8}");
        }
    }

    [TestMethod]
    public void Dema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<DemaResult> r = Qts.ToDema(15);

            DemaResult l = r[^1];
            Console.WriteLine($"DEMA(15) on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Dema:N8}");
        }
    }

    [TestMethod]
    public void Dynamic()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<DynamicResult> r = Qts.ToDynamic(100);

            DynamicResult l = r[^1];
            Console.WriteLine($"DYNAMIC(15) on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Dynamic:N8}");
        }
    }

    [TestMethod]
    public void Ema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<EmaResult> r = Qts.Cast<IReusable>().ToList().ToEma(15);

            EmaResult l = r[^1];
            Console.WriteLine($"EMA(15) on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Ema:N8}");
        }
    }

    [TestMethod]
    public void FisherTransform()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<FisherTransformResult> r = Qts.ToFisherTransform();

            FisherTransformResult l = r[^1];
            Console.WriteLine($"FT(10) on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Fisher:N8}");
        }
    }

    [TestMethod]
    public void Gator()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<GatorResult> r = Qts.ToGator();

            GatorResult l = r[^1];
            Console.WriteLine(
                "GATOR() on {0:d} with {1,4} periods: Upper {2:N8}  Lower {3:N8}",
                l.Timestamp, Qts.Count, l.Upper, l.Lower);
        }
    }

    [TestMethod]
    public void HtTrendline()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<HtlResult> r = Qts.ToHtTrendline();

            HtlResult l = r[^1];
            Console.WriteLine($"HTL on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Trendline:N8}");
        }
    }

    [TestMethod]
    public void Kama()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<KamaResult> r = Qts.ToKama();

            KamaResult l = r[^1];
            Console.WriteLine($"KAMA(10) on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Kama:N8}");
        }
    }

    [TestMethod]
    public void Keltner()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<KeltnerResult> r = Qts.ToKeltner(100);

            KeltnerResult l = r[^1];
            Console.WriteLine($"KC-UP on {l.Timestamp:d} with {Qts.Count,4} periods: {l.UpperBand:N8}");
        }
    }

    [TestMethod]
    public void Macd()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<MacdResult> r = Qts.ToMacd();

            MacdResult l = r[^1];
            Console.WriteLine($"MACD on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Macd:N8}");
        }
    }

    [TestMethod]
    public void Mama()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<MamaResult> r = Qts.ToMama();

            MamaResult l = r[^1];
            Console.WriteLine($"MAMA on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Mama:N8}");
        }
    }

    [TestMethod]
    public void Pmo()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<PmoResult> r = Qts.ToPmo();

            PmoResult l = r[^1];
            Console.WriteLine($"PMO on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Pmo:N8}");
        }
    }

    [TestMethod]
    public void Pvo()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<PvoResult> r = Qts.ToPvo();

            PvoResult l = r[^1];
            Console.WriteLine($"PVO on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Pvo:N8}");
        }
    }

    [TestMethod]
    public void Rsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<RsiResult> r = Qts.ToRsi();

            RsiResult l = r[^1];
            Console.WriteLine($"RSI(14) on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Rsi:N8}");
        }
    }

    [TestMethod]
    public void Smi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<SmiResult> r = Qts.ToSmi(14, 20, 5);

            SmiResult l = r[^1];
            Console.WriteLine($"SMI on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Smi:N8}");
        }
    }

    [TestMethod]
    public void Smma()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<SmmaResult> r = Qts.ToSmma(15);

            SmmaResult l = r[^1];
            Console.WriteLine($"SMMA(15) on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Smma:N8}");
        }
    }

    [TestMethod]
    public void StarcBands()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<StarcBandsResult> r = Qts.ToStarcBands(20);

            StarcBandsResult l = r[^1];
            Console.WriteLine($"STARC UPPER on {l.Timestamp:d} with {Qts.Count,4} periods: {l.UpperBand:N8}");
        }
    }

    [TestMethod]
    public void StochRsi()
    {
        foreach (int qty in QuotesQuantities.Where(x => x <= 502))
        {
            IReadOnlyList<StochRsiResult> r = Qts.ToStochRsi(14, 14, 3);

            StochRsiResult l = r[^1];
            Console.WriteLine($"SRSI on {l.Timestamp:d} with {Qts.Count,4} periods: {l.StochRsi:N8}");
        }
    }

    [TestMethod]
    public void T3()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<T3Result> r = Qts.ToT3(20);

            T3Result l = r[^1];
            Console.WriteLine($"T3 on {l.Timestamp:d} with {Qts.Count,4} periods: {l.T3:N8}");
        }
    }

    [TestMethod]
    public void Tema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<TemaResult> r = Qts.ToTema(15);

            TemaResult l = r[^1];
            Console.WriteLine($"TEMA on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Tema:N8}");
        }
    }

    [TestMethod]
    public void Trix()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<TrixResult> r = Qts.ToTrix(15);

            TrixResult l = r[^1];
            Console.WriteLine($"TRIX on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Trix:N8}");
        }
    }

    [TestMethod]
    public void Tsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<TsiResult> r = Qts.ToTsi();

            TsiResult l = r[^1];
            Console.WriteLine($"TSI on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Tsi:N8}");
        }
    }

    [TestMethod]
    public void Vortex()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<VortexResult> r = Qts.ToVortex(14);

            VortexResult l = r[^1];
            Console.WriteLine($"VI+ on {l.Timestamp:d} with {Qts.Count,4} periods: {l.Pvi:N8}");
        }
    }
}
