namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // STOCHASTIC OSCILLATOR
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<StochResult> GetStoch<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
        where TQuote : IQuote => quotes
            .GetStoch(
                lookbackPeriods,
                signalPeriods,
                smoothPeriods, 3, 2, MaType.SMA);

    /// <include file='./info.xml' path='indicator/type[@name="Extended"]/*' />
    ///
    public static IEnumerable<StochResult> GetStoch<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ToQuoteD();

        // check parameter arguments
        ValidateStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // initialize
        int length = quotesList.Count;
        List<StochResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotesList[i];

            StochResult result = new()
            {
                Date = q.Date
            };

            if (i + 1 >= lookbackPeriods)
            {
                double highHigh = double.MinValue;
                double lowLow = double.MaxValue;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD x = quotesList[p];

                    if (x.High > highHigh)
                    {
                        highHigh = x.High;
                    }

                    if (x.Low < lowLow)
                    {
                        lowLow = x.Low;
                    }
                }

                result.Oscillator = lowLow != highHigh
                    ? 100 * (q.Close - lowLow) / (highHigh - lowLow)
                    : 0;
            }

            results.Add(result);
        }

        // smooth the oscillator
        if (smoothPeriods > 1)
        {
            results = SmoothOscillator(
                results, length, lookbackPeriods, smoothPeriods, movingAverageType);
        }

        // handle insufficient length
        if (length < lookbackPeriods - 1)
        {
            return results;
        }

        // signal (%D) and %J
        int signalIndex = lookbackPeriods + smoothPeriods + signalPeriods - 2;
        double? s = results[lookbackPeriods - 1].Oscillator;

        for (int i = lookbackPeriods - 1; i < length; i++)
        {
            StochResult r = results[i];

            // add signal

            if (signalPeriods <= 1)
            {
                r.Signal = r.Oscillator;
            }

            // SMA case
            else if (i + 1 >= signalIndex && movingAverageType is MaType.SMA)
            {
                double? sumOsc = 0;
                for (int p = i + 1 - signalPeriods; p <= i; p++)
                {
                    StochResult x = results[p];
                    sumOsc += x.Oscillator;
                }

                r.Signal = sumOsc / signalPeriods;
            }

            // SMMA case
            else if (i >= lookbackPeriods - 1 && movingAverageType is MaType.SMMA)
            {
                s = (s == null) ? results[i].Oscillator : s; // reset if null

                s = ((s * (signalPeriods - 1)) + results[i].Oscillator) / signalPeriods;
                r.Signal = s;
            }

            // %J
            r.PercentJ = (kFactor * r.Oscillator) - (dFactor * r.Signal);
        }

        return results;
    }

    /// <include file='./info.xml' path='indicator/type[@name="Extended"]/*' />
    ///
    internal static IEnumerable<StochResult> GetStochJon<TQuote>(
       this IEnumerable<TQuote> quotes,
       int lookbackPeriods,
       int signalPeriods,
       int smoothPeriods,
       decimal kFactor,
       decimal dFactor,
       MaType movingAverageType)
       where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ToQuoteD();

        // check parameter arguments
        ValidateStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // initialize
        int length = quotesList.Count;
        List<StochResult> results = new(length);

        List<IQuote> ksma = new List<IQuote>();
        List<IQuote> dsma = new List<IQuote>();

        decimal lowestLow; // Lowest Low = lowest low for the look-back period
        decimal highestHigh; // Highest High = highest high for the look-back period
        decimal fastk = 0; // fastk = (Current Close - Lowest Low)/(Highest High - Lowest Low) * 100
        decimal? fullk = null; // fullk smoothed with X-period SMA
        decimal? fulld = null; // X-period SMA of Full fullk
        decimal? percentJ = null;

        // IEnumerable<TQuote> lookbackQuotes = quotes.Skip(lookbackPeriods).Take(lookbackPeriods);
        // IEnumerable<TQuote> lookbackQuotes = quotes.Skip(lookbackPeriods).TakeLast(lookbackPeriods);
        // lowestLow = quotes.Skip(pointIndex - (lookbackPeriods - 1)).Take(lookbackPeriods).OrderBy(sp => sp.Low).FirstOrDefault().Low.GetValueOrDefault();
        // highestHigh = quotes.Skip(pointIndex - (lookbackPeriods - 1)).Take(lookbackPeriods).OrderByDescending(sp => sp.High).FirstOrDefault().High.GetValueOrDefault();

        IEnumerable<TQuote> lookbackQuotes;
        int i = 0;
        foreach (QuoteD point in quotesList)
        {
            if (i >= lookbackPeriods)
            {
                lookbackQuotes = quotes.Skip(i + 1 - lookbackPeriods).Take(lookbackPeriods);
                lowestLow = lookbackQuotes.OrderBy(quote => quote.Low).First().Low.GetValueOrDefault();
                highestHigh = lookbackQuotes.OrderByDescending(sp => sp.High).First().High.GetValueOrDefault();

                fastk = ((decimal)point.Close.GetValueOrDefault() - lowestLow) / (highestHigh - lowestLow == 0 ? 1 : highestHigh - lowestLow) * 100;

                if (smoothPeriods > 1) // smooth the oscillator
                {
                    ksma.Add(new Quote()
                    {
                        Date = point.Date,
                        Close = fastk,
                    });
                    if (ksma.Count < smoothPeriods)
                    {
                        fullk = fastk;
                    }
                    else
                    {
                        fullk = ksma.GetSma(smoothPeriods, CandlePart.Close).LastOrDefault().Sma.GetValueOrDefault();
                    }
                }
                else
                {
                    fullk = fastk;
                }

                dsma.Add(new Quote()
                {
                    Date = point.Date,
                    Close = fullk,
                });
                if (dsma.Count < signalPeriods)
                {
                    fulld = fullk;
                }
                else
                {
                    fulld = dsma.GetSma(signalPeriods, CandlePart.Close).LastOrDefault().Sma.GetValueOrDefault();
                }

                percentJ = (kFactor * fullk) - (dFactor * fulld);
            }

            results.Add(new StochResult() { Date = point.Date, Oscillator = fullk, Signal = fulld, PercentJ = percentJ });

            i++;
        }

        if (length < lookbackPeriods - 1) // handle insufficient length
        {
            return results;
        }

        return results;
    }

    // internals
    private static List<StochResult> SmoothOscillator(
        List<StochResult> results,
        int length,
        int lookbackPeriods,
        int smoothPeriods,
        MaType movingAverageType)
    {
        // temporarily store interim smoothed oscillator
        double?[] smooth = new double?[length]; // smoothed value

        if (movingAverageType is MaType.SMA)
        {
            int smoothIndex = lookbackPeriods + smoothPeriods - 2;

            for (int i = smoothIndex; i < length; i++)
            {
                double? sumOsc = 0;
                for (int p = i + 1 - smoothPeriods; p <= i; p++)
                {
                    sumOsc += results[p].Oscillator;
                }

                smooth[i] = sumOsc / smoothPeriods;
            }
        }
        else if (movingAverageType is MaType.SMMA)
        {
            // initialize with unsmoothed value
            double? k = results[lookbackPeriods - 1].Oscillator;

            for (int i = lookbackPeriods - 1; i < length; i++)
            {
                k = (k == null) ? results[i].Oscillator : k; // reset if null

                k = ((k * (smoothPeriods - 1)) + results[i].Oscillator) / smoothPeriods;
                smooth[i] = k;
            }
        }
        else
        {
            return results;
        }

        // replace oscillator
        for (int i = 0; i < length; i++)
        {
            results[i].Oscillator = (smooth[i] != null) ? smooth[i] : null;
        }

        return results;
    }

    // parameter validation
    private static void ValidateStoch(
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Stochastic.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for Stochastic.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smooth periods must be greater than 0 for Stochastic.");
        }

        if (kFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kFactor), kFactor,
                "kFactor must be greater than 0 for Stochastic.");
        }

        if (dFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dFactor), dFactor,
                "dFactor must be greater than 0 for Stochastic.");
        }

        if (movingAverageType is not MaType.SMA and not MaType.SMMA)
        {
            throw new ArgumentOutOfRangeException(nameof(dFactor), dFactor,
                "Stochastic only supports SMA and SMMA moving average types.");
        }
    }
}
