using System.Reflection;

namespace BufferLists;

/// <summary>
/// A buffer list built on inner buffer lists bounds those inner lists with its own
/// <see cref="BufferList{TResult}.MaxListSize"/> without changing its results, down to
/// a <c>MaxListSize</c> of 1 where it is smaller than an inner list's lookback.
/// <c>StcList</c> and <c>AtrStopList</c> already bounded their inner lists; they are
/// here as regression guards.
/// </summary>
[TestClass]
public class BufferListInnerListsTests : TestBase
{
    private const int roomy = 50;

    // name -> (inner list field -> floor that list is held to, run for one MaxListSize)
    private static readonly Dictionary<string, (Dictionary<string, int> Floors, Action<int> Run)> Cases = new() {
        ["StochRsi"] = Case(new() { ["_rsiList"] = 15, ["_stochList"] = 15 },
            () => new StochRsiList(14, 14, 3, 1), (l, b) => l.Add(b), Bars.ToStochRsi(14, 14, 3, 1)),
        ["StochRsiSignal20"] = Case(new() { ["_rsiList"] = 15, ["_stochList"] = 21 },
            () => new StochRsiList(14, 3, 20, 3), (l, b) => l.Add(b), Bars.ToStochRsi(14, 3, 20, 3)),
        ["StochRsiSmooth3"] = Case(new() { ["_rsiList"] = 15, ["_stochList"] = 15 },
            () => new StochRsiList(14, 14, 3, 3), (l, b) => l.Add(b), Bars.ToStochRsi(14, 14, 3, 3)),
        ["Stc"] = Case(new() { ["_macdList"] = 51 },
            () => new StcList(10, 23, 50), (l, b) => l.Add(b), Bars.ToStc(10, 23, 50)),
        ["Keltner"] = Case(new() { ["_emaList"] = 21, ["_atrList"] = 11 },
            () => new KeltnerList(20, 2, 10), (l, b) => l.Add(b), Bars.ToKeltner(20, 2, 10)),
        ["Chandelier"] = Case(new() { ["_atrList"] = 23 },
            () => new ChandelierList(22, 3), (l, b) => l.Add(b), Bars.ToChandelier(22, 3)),
        ["AtrStop"] = Case(new() { ["_atrList"] = 22 },
            () => new AtrStopList(21, 3), (l, b) => l.Add(b), Bars.ToAtrStop(21, 3)),
        ["ChaikinOsc"] = Case(new() { ["_adlList"] = 2, ["_fastEmaList"] = 4, ["_slowEmaList"] = 11 },
            () => new ChaikinOscList(3, 10), (l, b) => l.Add(b), Bars.ToChaikinOsc(3, 10)),
        ["Cmf"] = Case(new() { ["_adlList"] = 2 },
            () => new CmfList(20), (l, b) => l.Add(b), Bars.ToCmf(20)),
        ["ElderRay"] = Case(new() { ["_emaList"] = 14 },
            () => new ElderRayList(13), (l, b) => l.Add(b), Bars.ToElderRay(13)),
        ["StarcBands"] = Case(new() { ["_smaList"] = 2, ["_atrList"] = 11 },
            () => new StarcBandsList(5, 2, 10), (l, b) => l.Add(b), Bars.ToStarcBands(5, 2, 10)),
        ["SuperTrend"] = Case(new() { ["_atrList"] = 11 },
            () => new SuperTrendList(10, 3), (l, b) => l.Add(b), Bars.ToSuperTrend(10, 3))
    };

    public static IEnumerable<object[]> Sizes =>
        Cases.SelectMany(c => new[] { 1, 2, Math.Max(3, c.Value.Floors.Values.Max() - 2), roomy }
            .Distinct()
            .Select(m => new object[] { c.Key, m }));

    [TestMethod]
    [DynamicData(nameof(Sizes))]
    public void MaxListSize_AtAnySize_KeepsResultsOfTheSeries(string name, int maxListSize)
        => Cases[name].Run(maxListSize);

    private static (Dictionary<string, int> Floors, Action<int> Run) Case<TList, TResult>(
        Dictionary<string, int> floors,
        Func<TList> make,
        Action<TList, Bar> add,
        IReadOnlyList<TResult> series)
        where TList : BufferList<TResult>
        where TResult : ISeries
        => (floors, m => Verify(m, floors, make, add, series));

    private static void Verify<TList, TResult>(
        int maxListSize,
        Dictionary<string, int> floors,
        Func<TList> make,
        Action<TList, Bar> add,
        IReadOnlyList<TResult> series)
        where TList : BufferList<TResult>
        where TResult : ISeries
    {
        int expectedCount = Math.Min(maxListSize, Bars.Count);

        void AssertTail(TList sut, string path)
        {
            sut.Should().HaveCount(expectedCount, path);
            sut.IsExactly(series.Skip(Bars.Count - expectedCount).ToList());

            // every inner buffer list is listed with its floor, and held to the larger of that and the outer bound
            List<FieldInfo> inner = typeof(TList)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => IsBufferList(f.FieldType))
                .ToList();

            inner.Select(f => f.Name).Should().BeEquivalentTo(floors.Keys, $"{path} inner lists");

            foreach (FieldInfo field in inner)
            {
                object list = field.GetValue(sut)!;
                int count = (int)list.GetType().GetProperty("Count")!.GetValue(list)!;

                count.Should().BeLessThanOrEqualTo(
                    Math.Max(maxListSize + 1, floors[field.Name]),
                    $"{path} {field.Name}");
            }
        }

        // limit set before any data
        TList sut = make();
        sut.MaxListSize = maxListSize;

        foreach (Bar bar in Bars)
        {
            add(sut, bar);
        }

        AssertTail(sut, "incremental");

        // refill after Clear
        sut.Clear();

        foreach (Bar bar in Bars)
        {
            add(sut, bar);
        }

        AssertTail(sut, "clear and refill");

        // limit set after the data is loaded
        TList late = make();

        foreach (Bar bar in Bars)
        {
            add(late, bar);
        }

        late.MaxListSize = maxListSize;
        AssertTail(late, "limit set after load");
    }

    private static bool IsBufferList(Type type)
    {
        for (Type t = type; t != null; t = t.BaseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BufferList<>))
            {
                return true;
            }
        }

        return false;
    }
}
