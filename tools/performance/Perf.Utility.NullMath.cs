namespace Performance;

[Config(typeof(MicrotestConfig))]
[WarmupCount(5), IterationCount(10), IterationTime(1000)]
public class UtilityNullMath
{
    private static readonly double? dblVal = 54321.0123456789d;
    private static readonly double? dblNul = null;
    private static readonly decimal? decVal = 54321.0123456789m;
    private static readonly decimal? decNul = null;
    private static readonly double? nulNaN = double.NaN;
    private const double dblNaN = double.NaN;
    private const int OpsQty = 8;

    // Abs()

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double? AbsDblVal() => dblVal.Abs();

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double? AbsDblNul() => dblNul.Abs();

    // Null2NaN()

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double Null2NaNDecVal() => decVal.Null2NaN();

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double Null2NaNDecNul() => decNul.Null2NaN();

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double Null2NaNDblVal() => dblVal.Null2NaN();

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double Null2NaNDblNul() => dblNul.Null2NaN();

    // Nan2Null()

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double? NaN2NullDblVal() => dblVal.NaN2Null();

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double? NaN2NullDblNul() => dblNul.NaN2Null();

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double? NaN2NullNaNVal() => dblNaN.NaN2Null();

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public double? NaN2NullNanNul() => nulNaN.NaN2Null();
}
