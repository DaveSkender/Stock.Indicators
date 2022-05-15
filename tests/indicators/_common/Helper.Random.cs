namespace Skender.Stock.Indicators;

/**

GBM - Geometric Brownian Motion is a random simulator of market movement, returning List<Quote> 
    GBM can be used for testing indicators, validation and Monte Carlo simulations of strategies. 

    Sample usage:
    GBM-Random data = new(); // generates 1 year (252) list of bars
    GBM-Random data = new(Bars: 1000); // generates 1,000 bars
    GBM-Random data = new(Bars: 252, Volatility: 0.05, Drift: 0.0005, Seed: 100.0)

    Parameters
    Bars:       number of bars (quotes) requested
    Volatility: how dymamic/volatile the series should be; default is 1
    Drift:      incremental drift due to annual interest rate; default is 5%
    Seed:       starting value of the random series; should not be 0

**/
internal class GBM_Random : List<Quote>
{
    private double seed;
    private readonly double volatility;
    private readonly double drift;

    public GBM_Random(int Bars = 252, double Volatility = 1.0, double Drift = 0.05, double Seed = 100.0)
    {
        seed = Seed;
        volatility = Volatility * 0.01;
        drift = Drift * 0.01;
        for (int i = 0; i < Bars; i++)
        {
            DateTime Timestamp = DateTime.Today.AddDays(i - Bars);
            Add(Timestamp);
        }
    }

    public void Add(DateTime timestamp)
    {
        double Open = GBM_value(seed, volatility * volatility, drift);
        double Close = GBM_value(Open, volatility, drift);

        double OCMax = Math.Max(Open, Close);
        double High = GBM_value(seed, volatility * 0.5, 0);
        High = (High < OCMax) ? (2 * OCMax) - High : High;

        double OCMin = Math.Min(Open, Close);
        double Low = GBM_value(seed, volatility * 0.5, 0);
        Low = (Low > OCMin) ? (2 * OCMin) - Low : Low;

        double Volume = GBM_value(seed * 10, volatility * 2, Drift: 0);

        Quote item = new() { Date = timestamp, Open = (decimal)Open, High = (decimal)High, Low = (decimal)Low, Close = (decimal)Close, Volume = (decimal)Volume };
        base.Add(item);
        seed = Close;
    }

    private double GBM_value(double Seed, double Volatility, double Drift)
    {
        Random rnd = new((int)DateTime.UtcNow.Ticks);
        double U1 = 1.0 - rnd.NextDouble();
        double U2 = 1.0 - rnd.NextDouble();
        double Z = Math.Sqrt(-2.0 * Math.Log(U1)) * Math.Sin(2.0 * Math.PI * U2);
        return Seed * Math.Exp(Drift - (Volatility * Volatility * 0.5) + (Volatility * Z));
    }
}