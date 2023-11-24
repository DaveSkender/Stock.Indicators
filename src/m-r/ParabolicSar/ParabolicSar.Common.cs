using System.Globalization;

namespace Skender.Stock.Indicators;

// PARABOLIC SAR (COMMON)

public static class ParabolicSar
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);

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

}
