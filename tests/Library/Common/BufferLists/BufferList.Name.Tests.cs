namespace BufferLists;

/// <summary>
/// A buffer list's name reports the arguments it was built with, not the defaults.
/// </summary>
[TestClass]
public class BufferListNameTests : TestBase
{
    public static IEnumerable<object[]> Lists =>
    [
        [new AlmaList(10, 0.5, 4), "ALMA(10, 0.5, 4)"],
        [new AtrStopList(14, 2.5, EndType.HighLow), "ATRSTOP(14, 2.5, HighLow)"],
        [new BollingerBandsList(10, 1.5), "BOLLINGERBANDS(10, 1.5)"],
        [new ChandelierList(10, 2, Direction.Short), "CHANDELIER(10, 2, Short)"],
        [new DynamicList(10, 0.5), "DYNAMIC(10, 0.5)"],
        [new FractalList(3, 4, EndType.Close), "FRACTAL(3, 4, Close)"],
        [new RenkoList(2.5m, EndType.HighLow), "RENKO(2.5, HighLow)"],
        [new RollingPivotsList(10, 2, PivotPointType.Camarilla), "ROLLINGPIVOTS(10, 2, Camarilla)"],
        [new StochList(9, 2, 4, 2, 1, MaType.SMMA), "STOCH(9, 2, 4, 2, 1, SMMA)"],
        [new StochRsiList(10, 12, 2, 3), "STOCHRSI(10, 12, 2, 3)"],
        [new T3List(6, 0.5), "T3(6, 0.5)"],
    ];

    [TestMethod]
    [DynamicData(nameof(Lists))]
    public void Name_ReportsConstructorArguments(object list, string expected)
    {
        ArgumentNullException.ThrowIfNull(list);
        list.ToString().Should().Be(expected);
    }
}
