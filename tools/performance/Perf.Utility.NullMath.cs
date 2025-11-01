namespace Tests.Performance;

#pragma warning disable CA1805 // Do not initialize unnecessarily

[ShortRunJob]
public class UtilityNullMath
{
    private static readonly double? dblVal = 54321.0123456789d;
    private static readonly double? dblNul = null;
    private static readonly decimal? decVal = 54321.0123456789m;
    private static readonly decimal? decNul = null;
    private static readonly double? nulNaN = double.NaN;
    private const double dblNaN = double.NaN;

    // Abs()

    [Benchmark]
    public double? AbsDblVal() => dblVal.Abs();

    [Benchmark]
    public double? AbsDblNul() => dblNul.Abs();

    // Round()

    [Benchmark]
    public decimal? RoundDecVal() => decVal.Round(2);

    [Benchmark]
    public decimal? RoundDecNul() => decNul.Round(2);

    [Benchmark]
    public double? RoundDblVal() => dblVal.Round(2);

    [Benchmark]
    public double? RoundDblNul() => dblNul.Round(2);

    // Null2NaN()

    [Benchmark]
    public double Null2NaNDecVal() => decVal.Null2NaN();

    [Benchmark]
    public double Null2NaNDecNul() => decNul.Null2NaN();

    [Benchmark]
    public double Null2NaNDblVal() => dblVal.Null2NaN();

    [Benchmark]
    public double Null2NaNDblNul() => dblNul.Null2NaN();

    // Nan2Null()

    [Benchmark]
    public double? NaN2NullDblVal() => dblVal.NaN2Null();

    [Benchmark]
    public double? NaN2NullDblNul() => dblNul.NaN2Null();

    [Benchmark]
    public double? NaN2NullNaNVal() => dblNaN.NaN2Null();

    [Benchmark]
    public double? NaN2NullNanNul() => nulNaN.NaN2Null();
}
