namespace Skender.Stock.Indicators;

// MOTHER of ADAPTIVE MOVING AVERAGES (STREAMING)

public partial class Mama
{
    // TBD: constructor
    public Mama()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        double fastLimit,
        double slowLimit)
    {
        // check parameter arguments
        if (fastLimit <= slowLimit || fastLimit >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(fastLimit), fastLimit,
                "Fast Limit must be greater than Slow Limit and less than 1 for MAMA.");
        }

        if (slowLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slowLimit), slowLimit,
                "Slow Limit must be greater than 0 for MAMA.");
        }
    }

    // TBD: increment  calculation
    internal static double Increment() => throw new NotImplementedException();

    // NON-STATIC METHODS

    // handle quote arrival
    public virtual void OnNext((DateTime Date, double Value) value)
    {
    }

    // TBD: initialize with existing quote cache
    private void Initialize() => throw new NotImplementedException();
}
