using System.Globalization;

namespace Skender.Stock.Indicators;

// PARABOLIC SAR (STREAMING)

public partial class ParabolicSar
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);

    // TBD: constructor
    public ParabolicSar()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
    {
        // check parameter arguments
        if (accelerationStep <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(accelerationStep), accelerationStep,
                "Acceleration Step must be greater than 0 for Parabolic SAR.");
        }

        if (maxAccelerationFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAccelerationFactor), maxAccelerationFactor,
                "Max Acceleration Factor must be greater than 0 for Parabolic SAR.");
        }

        if (accelerationStep > maxAccelerationFactor)
        {
            string message = string.Format(
                EnglishCulture,
                "Acceleration Step cannot be larger than the Max Acceleration Factor ({0}) for Parabolic SAR.",
                maxAccelerationFactor);

            throw new ArgumentOutOfRangeException(nameof(accelerationStep), accelerationStep, message);
        }

        if (initialFactor <= 0 || initialFactor > maxAccelerationFactor)
        {
            throw new ArgumentOutOfRangeException(nameof(initialFactor), initialFactor,
                "Initial Factor must be greater than 0 and not larger than Max Acceleration Factor for Parabolic SAR.");
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
