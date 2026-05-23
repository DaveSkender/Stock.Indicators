namespace Skender.Stock.Indicators;

public static class Numerix
{
    // STANDARD DEVIATION
    public static double StdDev(this double[] values)
    {
        // validate parameters
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values), "StdDev values cannot be null.");
        }

        double sd = 0;
        int n = values.Length;
        if (n > 1)
        {
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += values[i];
            }

            double avg = sum / n;

            double sumSq = 0;
            for (int i = 0; i < n; i++)
            {
                double v = values[i];
                sumSq += (v - avg) * (v - avg);
            }

            sd = Math.Sqrt(sumSq / n);
        }

        return sd;
    }

    // SLOPE of BEST FIT LINE
    public static double Slope(double[] x, double[] y)
    {
        // validate parameters
        if (x is null)
        {
            throw new ArgumentNullException(nameof(x), "Slope X values cannot be null.");
        }

        if (y is null)
        {
            throw new ArgumentNullException(nameof(y), "Slope Y values cannot be null.");
        }

        if (x.Length != y.Length)
        {
            throw new ArgumentException("Slope X and Y arrays must be the same size.");
        }

        int length = x.Length;

        // get averages for period
        double sumX = 0;
        double sumY = 0;

        for (int i = 0; i < length; i++)
        {
            sumX += x[i];
            sumY += y[i];
        }

        double avgX = sumX / length;
        double avgY = sumY / length;

        // least squares method
        double sumSqX = 0;
        double sumSqXy = 0;

        for (int i = 0; i < length; i++)
        {
            double devX = x[i] - avgX;
            double devY = y[i] - avgY;

            sumSqX += devX * devX;
            sumSqXy += devX * devY;
        }

        double slope = sumSqXy / sumSqX;

        return slope;
    }

    // LOG GAMMA (Lanczos approximation)
    // Accurate for x > 0; valid range documented below.
    // Uses reflection formula for 0 < x < 0.5, Lanczos for x >= 0.5.
    internal static double LogGamma(double x)
    {
        if (x <= 0)
        {
            return double.NaN;
        }

        double[] c = [
            0.99999999999980993,
            676.5203681218851,
            -1259.1392167224028,
            771.32342877765313,
            -176.61502916214059,
            12.507343278686905,
            -0.13857109526572012,
            9.9843695780195716e-6,
            1.5056327351493116e-7
        ];

        if (x < 0.5)
        {
            // reflection formula: Γ(x)Γ(1-x) = π/sin(πx)
            double sinPiX = Math.Sin(Math.PI * x);
            return sinPiX > 0
                ? Math.Log(Math.PI / sinPiX) - LogGamma(1.0 - x)
                : double.NaN;
        }

        x -= 1.0;
        double a = c[0];
        double t = x + 7.5; // g + 0.5, where g = 7

        for (int i = 1; i <= 8; i++)
        {
            a += c[i] / (x + i);
        }

        return 0.5 * Math.Log(2 * Math.PI) + (x + 0.5) * Math.Log(t) - t + Math.Log(a);
    }

    // DATE ROUNDING
    internal static DateTime RoundDown(this DateTime dateTime, TimeSpan interval)
        => interval == TimeSpan.Zero
        ? dateTime
        : dateTime
            .AddTicks(-(dateTime.Ticks % interval.Ticks));

    // PERIOD-SIZE to TIMESPAN CONVERSION
    internal static TimeSpan ToTimeSpan(this PeriodSize periodSize)
        => periodSize switch {
            PeriodSize.OneMinute => TimeSpan.FromMinutes(1),
            PeriodSize.TwoMinutes => TimeSpan.FromMinutes(2),
            PeriodSize.ThreeMinutes => TimeSpan.FromMinutes(3),
            PeriodSize.FiveMinutes => TimeSpan.FromMinutes(5),
            PeriodSize.FifteenMinutes => TimeSpan.FromMinutes(15),
            PeriodSize.ThirtyMinutes => TimeSpan.FromMinutes(30),
            PeriodSize.OneHour => TimeSpan.FromHours(1),
            PeriodSize.TwoHours => TimeSpan.FromHours(2),
            PeriodSize.FourHours => TimeSpan.FromHours(4),
            PeriodSize.Day => TimeSpan.FromDays(1),
            PeriodSize.Week => TimeSpan.FromDays(7),
            _ => TimeSpan.Zero
        };

    // DETERMINE DECIMAL PLACES
    internal static int GetDecimalPlaces(this decimal n)
    {
        // source: https://stackoverflow.com/a/30205131/4496145

        n = Math.Abs(n); // make sure it is positive.
        n -= (int)n;     // remove the integer part of the number.
        int decimalPlaces = 0;
        while (n > 0)
        {
            decimalPlaces++;
            n *= 10;
            n -= (int)n;
        }

        return decimalPlaces;
    }
}
