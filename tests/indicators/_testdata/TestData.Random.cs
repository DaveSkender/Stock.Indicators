namespace Test.Data;

/// <summary>
/// Geometric Brownian Motion (GMB) is a random simulator of market movement.
/// GBM can be used for testing indicators, validation and Monte Carlo simulations of strategies.
/// </summary>
internal class RandomGbm : List<Quote>
{
    private readonly double _volatility;
    private readonly double _drift;
    private double _seed;
    private static readonly Random _random = new((int)DateTime.UtcNow.Ticks);

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomGbm"/> class.
    /// <code>
    /// Sample usage:
    ///
    /// RandomGbm data = new(); // generates 1 year (252) list of bars
    /// RandomGbm data = new(Bars: 1000); // generates 1,000 bars
    /// RandomGbm data = new(Bars: 252, Volatility: 0.05, Drift: 0.0005, Seed: 100.0)
    /// </code>
    /// </summary>
    /// <param name="bars">Number of bars (quotes) requested.</param>
    /// <param name="volatility">How dynamic/volatile the series should be; default is 1.</param>
    /// <param name="drift">Incremental drift due to annual interest rate; default is 5%.</param>
    /// <param name="seed">Starting value of the random series; should not be 0.</param>
    /// <param name="periodSize">The period size for the quotes.</param>
    /// <param name="includeWeekends">Whether to include weekends in the generated data.</param>
    /// <exception cref="ArgumentException">Thrown when an invalid argument is provided.</exception>
    public RandomGbm(
        int bars = 250,
        double volatility = 1.0,
        double drift = 0.01,
        double seed = 1000.0,
        PeriodSize periodSize = PeriodSize.OneMinute,
        bool includeWeekends = true)
    {
        // validation
        if (bars <= 0)
        {
            throw new ArgumentException("Number of bars must be greater than zero.", nameof(bars));
        }

        if (volatility <= 0)
        {
            throw new ArgumentException("Volatility must be greater than zero.", nameof(volatility));
        }

        if (seed <= 0)
        {
            throw new ArgumentException("Seed must be greater than zero.", nameof(seed));
        }

        TimeSpan frequency = periodSize.ToTimeSpan();

        if (!includeWeekends && (frequency < TimeSpan.FromHours(1) || frequency >= TimeSpan.FromDays(7)))
        {
            throw new ArgumentException("Weekends can only be excluded for period sizes between OneHour and OneWeek.", nameof(includeWeekends));
        }

        _seed = seed;
        _volatility = volatility * 0.01;
        _drift = drift * 0.001;

        DateTime date = DateTime.Today.Add(frequency * -bars);
        int generatedBars = 0;

        while (generatedBars < bars)
        {
            if (includeWeekends || frequency < TimeSpan.FromHours(1) || frequency >= TimeSpan.FromDays(7) || (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday))
            {
                Add(date);
                generatedBars++;
            }

            date = date.Add(frequency);
        }
    }

    /// <summary>
    /// Adds a new quote to the list.
    /// </summary>
    /// <param name="timestamp">The timestamp of the quote.</param>
    public void Add(DateTime timestamp)
    {
        double open = Price(_seed, _volatility * _volatility, _drift);
        double close = Price(open, _volatility, _drift);

        double ocMax = Math.Max(open, close);
        double high = Price(_seed, _volatility * 0.5, 0);
        high = high < ocMax ? (2 * ocMax) - high : high;

        double ocMin = Math.Min(open, close);
        double low = Price(_seed, _volatility * 0.5, 0);
        low = low > ocMin ? (2 * ocMin) - low : low;

        double volume = Price(_seed * 1000, _volatility * 2, drift: 0);

        Quote quote = new(
            Timestamp: timestamp,
            Open: (decimal)open,
            High: (decimal)high,
            Low: (decimal)low,
            Close: (decimal)close,
            Volume: (decimal)volume);

        Add(quote);
        _seed = close;
    }

    /// <summary>
    /// Generates a random price based on the seed, volatility, and drift.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <param name="volatility">The volatility value.</param>
    /// <param name="drift">The drift value.</param>
    /// <returns>A random price.</returns>
    private static double Price(double seed, double volatility, double drift)
    {
        double u1 = 1.0 - _random.NextDouble();
        double u2 = 1.0 - _random.NextDouble();
        double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return seed * Math.Exp(drift - (volatility * volatility * 0.5) + (volatility * z));
    }
}
