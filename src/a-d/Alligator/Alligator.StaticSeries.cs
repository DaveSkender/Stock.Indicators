namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Williams Alligator indicator.
/// </summary>
public static partial class Alligator
{
    /// <summary>
    /// Calculates the Williams Alligator indicator for a series of data.
    /// </summary>
    /// <typeparam name="T">
    /// <c>T</c> must be <see cref="IReusable"/> or <see cref="IQuote"/> type
    /// </typeparam>
    /// <param name="source">Time-series values to transform.</param>
    /// <param name="jawPeriods">Lookback periods for the Jaw line.</param>
    /// <param name="jawOffset">Offset periods for the Jaw line.</param>
    /// <param name="teethPeriods">Lookback periods for the Teeth line.</param>
    /// <param name="teethOffset">Offset periods for the Teeth line.</param>
    /// <param name="lipsPeriods">Lookback periods for the Lips line.</param>
    /// <param name="lipsOffset">Offset periods for the Lips line.</param>
    /// <returns>Time series of Alligator values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Invalid parameter value provided.
    /// </exception>
    [Series("ALLIGATOR", "Williams Alligator", Category.PriceTrend, ChartType.Overlay)]
    public static IReadOnlyList<AlligatorResult> ToAlligator<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Jaw Periods", 13, 1, 250)]
        int jawPeriods = 13,
        [ParamNum<int>("Jaw Offset", 8, 1, 30)]
        int jawOffset = 8,
        [ParamNum<int>("Teeth Periods", 8, 1, 250)]
        int teethPeriods = 8,
        [ParamNum<int>("Teeth Offset", 5, 1, 30)]
        int teethOffset = 5,
        [ParamNum<int>("Lips Periods", 5, 1, 250)]
        int lipsPeriods = 5,
        [ParamNum<int>("Lips Offset", 3, 1, 30)]
        int lipsOffset = 3)
        where T : IReusable
    {
        // check parameter arguments
        Validate(
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);

        // prefer HL2 when IQuote
        IReadOnlyList<IReusable> values
            = source.ToPreferredList(CandlePart.HL2);

        // initialize
        int length = values.Count;
        List<AlligatorResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            double jaw = double.NaN;
            double lips = double.NaN;
            double teeth = double.NaN;

            // calculate alligator's jaw, when in range
            if (i >= jawPeriods + jawOffset - 1)
            {
                // first/reset value: calculate SMA
                if (results[i - 1].Jaw is null)
                {
                    double sum = 0;
                    for (int p = i - jawPeriods - jawOffset + 1; p <= i - jawOffset; p++)
                    {
                        sum += values[p].Value;
                    }

                    jaw = sum / jawPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double prevJaw = results[i - 1].Jaw.Null2NaN();

                    jaw = ((prevJaw * (jawPeriods - 1)) + values[i - jawOffset].Value) / jawPeriods;
                }
            }

            // calculate alligator's teeth, when in range
            if (i >= teethPeriods + teethOffset - 1)
            {
                // first/reset value: calculate SMA
                if (results[i - 1].Teeth is null)
                {
                    double sum = 0;
                    for (int p = i - teethPeriods - teethOffset + 1; p <= i - teethOffset; p++)
                    {
                        sum += values[p].Value;
                    }

                    teeth = sum / teethPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double prevTooth = results[i - 1].Teeth.Null2NaN();

                    teeth = ((prevTooth * (teethPeriods - 1)) + values[i - teethOffset].Value) / teethPeriods;
                }
            }

            // calculate alligator's lips, when in range
            if (i >= lipsPeriods + lipsOffset - 1)
            {
                // first/reset value: calculate SMA
                if (results[i - 1].Lips is null)
                {
                    double sum = 0;
                    for (int p = i - lipsPeriods - lipsOffset + 1; p <= i - lipsOffset; p++)
                    {
                        sum += values[p].Value;
                    }

                    lips = sum / lipsPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double prevLips = results[i - 1].Lips.Null2NaN();

                    lips = ((prevLips * (lipsPeriods - 1)) + values[i - lipsOffset].Value) / lipsPeriods;
                }
            }

            // result
            results.Add(new AlligatorResult(
                values[i].Timestamp,
                jaw.NaN2Null(),
                teeth.NaN2Null(),
                lips.NaN2Null()));
        }

        return results;
    }
}
