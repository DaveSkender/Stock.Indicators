namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Alligator indicator calculation.
/// </summary>
/// <param name="Timestamp">The date and time of the record.</param>
/// <param name="Jaw">The value of the Jaw line of the Alligator indicator.</param>
/// <param name="Teeth">The value of the Teeth line of the Alligator indicator.</param>
/// <param name="Lips">The value of the Lips line of the Alligator indicator.</param>
[Serializable]
public record AlligatorResult
(
    DateTime Timestamp,
    double? Jaw,
    double? Teeth,
    double? Lips
) : IReusable
{
    public double Value => double.NaN;
}
