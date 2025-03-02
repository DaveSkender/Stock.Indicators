#pragma warning disable CA1008 // Enums should have zero value
// TODO: fix or add zero value

[Serializable]
public enum Order
{
    Front = 1,
    Behind = 50,
    // price is 75/76
    BehindPrice = 80,
    Back = 95
    // thresholds are 99
}
