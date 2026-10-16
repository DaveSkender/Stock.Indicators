namespace BufferLists;

/// <summary>
/// Pins the chronological-input contract for <see cref="BufferList{TResult}"/>:
/// buffer lists are single-pass accumulators that follow input order verbatim.
/// They do not reorder, deduplicate, or correct out-of-order input — callers
/// must pre-sort, or use a <c>StreamHub</c> for out-of-order handling.
/// </summary>
/// <remarks>
/// These are characterization tests. If a future change makes enforcement the
/// default or adds silent reordering, they will fail and flag the contract
/// shift for deliberate review. Enforcement is opt-in through
/// <see cref="BufferList{TResult}.ThrowOnOutOfOrder"/>. The shipped behavior is
/// documented prominently on <c>BufferList{TResult}</c>, the increment
/// interfaces, and the buffer-list guide.
/// </remarks>
[TestClass]
public class OrderingContract : TestBase
{
    [TestMethod]
    public void OrderedInput_MatchesSeriesOracle()
    {
        // Positive control: chronological input reproduces the batch series exactly.
        IReadOnlyList<Bar> bars = Bars.Take(60).ToList();

        SmaList sut = new(20);
        sut.Add(bars);

        sut.IsExactly(bars.ToSma(20));
    }

    [TestMethod]
    public void OutOfOrderInput_IsAppendedVerbatim_NotSortedOrCorrected()
    {
        // Swap two adjacent bars to feed an out-of-order series.
        IReadOnlyList<Bar> bars = Bars.Take(60).ToList();
        List<Bar> scrambled = [.. bars];
        (scrambled[40], scrambled[41]) = (scrambled[41], scrambled[40]);

        SmaList sut = new(20);
        sut.Add(scrambled);

        // The buffer preserves input order verbatim; it neither sorts nor drops.
        sut.Should().HaveCount(scrambled.Count);
        sut[40].Timestamp.Should().Be(scrambled[40].Timestamp);
        sut[41].Timestamp.Should().Be(scrambled[41].Timestamp);

        // Consequently the result timeline is non-chronological — exactly the
        // silent-incorrectness the documented precondition warns about.
        bool resultTimelineIsChronological = sut
            .Zip(sut.Skip(1), static (a, b) => a.Timestamp <= b.Timestamp)
            .All(static ordered => ordered);

        resultTimelineIsChronological.Should().BeFalse(
            "a buffer list follows input order verbatim and never reorders out-of-order input");
    }

    [TestMethod]
    public void DuplicateTimestamp_AppendsSecondRow_DoesNotCorrect()
    {
        // Unlike a StreamHub (which treats a same-timestamp re-send as a
        // correction/rollback), a buffer list appends a second row.
        IReadOnlyList<Bar> bars = Bars.Take(30).ToList();

        SmaList sut = new(20);
        sut.Add(bars);
        int countBeforeResend = sut.Count;

        sut.Add(bars[^1]); // re-send the latest bar (same timestamp)

        sut.Should().HaveCount(countBeforeResend + 1);
        sut[^1].Timestamp.Should().Be(bars[^1].Timestamp);
        sut[^2].Timestamp.Should().Be(bars[^1].Timestamp);
    }

    [TestMethod]
    public void Strict_OrderedInput_DoesNotThrow()
    {
        IReadOnlyList<Bar> bars = Bars.Take(60).ToList();

        SmaList sut = new(20) { ThrowOnOutOfOrder = true };
        sut.Add(bars);

        sut.IsExactly(bars.ToSma(20));
    }

    [TestMethod]
    public void Strict_OutOfOrderInput_Throws()
    {
        IReadOnlyList<Bar> bars = Bars.Take(60).ToList();

        SmaList sut = new(20) { ThrowOnOutOfOrder = true };
        sut.Add(bars);

        Action add = () => sut.Add(bars[^2]);

        add.Should().Throw<InvalidOperationException>()
            .WithMessage("*not after the last timestamp*StreamHub*");
    }

    [TestMethod]
    public void Strict_DuplicateTimestamp_Throws()
    {
        IReadOnlyList<Bar> bars = Bars.Take(30).ToList();

        SmaList sut = new(20) { ThrowOnOutOfOrder = true };
        sut.Add(bars);

        Action add = () => sut.Add(bars[^1]);

        add.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Strict_AfterPruning_ComparesAgainstNewestRetained()
    {
        IReadOnlyList<Bar> bars = Bars.Take(60).ToList();

        SmaList sut = new(5) { MaxListSize = 3, ThrowOnOutOfOrder = true };
        sut.Add(bars);

        sut.Should().HaveCount(3);
        Action stale = () => sut.Add(bars[10]);
        stale.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Strict_AfterClear_AcceptsEarlierInput()
    {
        IReadOnlyList<Bar> bars = Bars.Take(30).ToList();

        SmaList sut = new(20) { ThrowOnOutOfOrder = true };
        sut.Add(bars);
        sut.Clear();

        Action restart = () => sut.Add(bars);

        restart.Should().NotThrow();
    }

    [TestMethod]
    public void Strict_Renko_AllowsBricksSharingATimestamp()
    {
        // one bar can complete several bricks, so repeated timestamps are valid
        RenkoList sut = new(1m) { ThrowOnOutOfOrder = true };

        Action feed = () => sut.Add(Bars);

        feed.Should().NotThrow();
        sut.Select(static r => r.Timestamp).Distinct().Count()
            .Should().BeLessThan(sut.Count);
    }
}
