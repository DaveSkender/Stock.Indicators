using Skender.Stock.Indicators;

namespace Tests.Indicators.Baselines;

#pragma warning disable CA1707 // Test method naming convention uses underscores for readability

[TestClass]
public class BaselineComparerTests : TestBase
{
    [TestMethod]
    public void IdenticalResults_ReturnsMatch()
    {
        // Arrange - Create identical results
        List<SmaResult> expected =
        [
            new(new DateTime(2021, 1, 1), 100.0),
            new(new DateTime(2021, 1, 2), 101.0),
            new(new DateTime(2021, 1, 3), 102.0)
        ];

        List<SmaResult> actual =
        [
            new(new DateTime(2021, 1, 1), 100.0),
            new(new DateTime(2021, 1, 2), 101.0),
            new(new DateTime(2021, 1, 3), 102.0)
        ];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeTrue();
        result.Mismatches.Should().BeEmpty();
    }

    [TestMethod]
    public void DifferentValues_ReturnsMismatch()
    {
        // Arrange
        List<SmaResult> expected =
        [
            new(new DateTime(2021, 1, 1), 100.0),
            new(new DateTime(2021, 1, 2), 101.0)
        ];

        List<SmaResult> actual =
        [
            new(new DateTime(2021, 1, 1), 100.0),
            new(new DateTime(2021, 1, 2), 101.5) // Different value
        ];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeFalse();
        result.Mismatches.Should().HaveCount(1);

        MismatchDetail mismatch = result.Mismatches[0];
        mismatch.Timestamp.Should().Be(new DateTime(2021, 1, 2));
        mismatch.PropertyName.Should().Be("Sma");
        mismatch.Expected.Should().Be(101.0);
        mismatch.Actual.Should().Be(101.5);
        mismatch.Delta.Should().Be(0.5);
    }

    [TestMethod]
    public void NullValues_BothNull_ReturnsMatch()
    {
        // Arrange
        List<SmaResult> expected =
        [
            new(new DateTime(2021, 1, 1), null),
            new(new DateTime(2021, 1, 2), null)
        ];

        List<SmaResult> actual =
        [
            new(new DateTime(2021, 1, 1), null),
            new(new DateTime(2021, 1, 2), null)
        ];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeTrue();
        result.Mismatches.Should().BeEmpty();
    }

    [TestMethod]
    public void NullValues_OnlyOneNull_ReturnsMismatch()
    {
        // Arrange
        List<SmaResult> expected =
        [
            new(new DateTime(2021, 1, 1), null),
            new(new DateTime(2021, 1, 2), 101.0)
        ];

        List<SmaResult> actual =
        [
            new(new DateTime(2021, 1, 1), 100.0), // Null in expected, value in actual
            new(new DateTime(2021, 1, 2), null)   // Value in expected, null in actual
        ];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeFalse();
        result.Mismatches.Should().HaveCount(2);

        // Check first mismatch (null vs value)
        MismatchDetail mismatch1 = result.Mismatches[0];
        mismatch1.Timestamp.Should().Be(new DateTime(2021, 1, 1));
        mismatch1.Expected.Should().BeNull();
        mismatch1.Actual.Should().Be(100.0);

        // Check second mismatch (value vs null)
        MismatchDetail mismatch2 = result.Mismatches[1];
        mismatch2.Timestamp.Should().Be(new DateTime(2021, 1, 2));
        mismatch2.Expected.Should().Be(101.0);
        mismatch2.Actual.Should().BeNull();
    }

    [TestMethod]
    public void CountMismatch_ReturnsMismatch()
    {
        // Arrange
        List<SmaResult> expected =
        [
            new(new DateTime(2021, 1, 1), 100.0),
            new(new DateTime(2021, 1, 2), 101.0),
            new(new DateTime(2021, 1, 3), 102.0)
        ];

        List<SmaResult> actual =
        [
            new(new DateTime(2021, 1, 1), 100.0),
            new(new DateTime(2021, 1, 2), 101.0)
        ];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeFalse();
        result.Mismatches.Should().HaveCount(1);

        MismatchDetail mismatch = result.Mismatches[0];
        mismatch.PropertyName.Should().Be("_COUNT_");
        mismatch.Expected.Should().Be(3);
        mismatch.Actual.Should().Be(2);
        mismatch.Delta.Should().Be(1);
    }

    [TestMethod]
    public void MultiPropertyIndicator_AllMatch_ReturnsMatch()
    {
        // Arrange - Using MacdResult which has multiple properties
        List<MacdResult> expected =
        [
            new(new DateTime(2021, 1, 1), 1.0, 2.0, 3.0, 4.0, 5.0),
            new(new DateTime(2021, 1, 2), 1.1, 2.1, 3.1, 4.1, 5.1)
        ];

        List<MacdResult> actual =
        [
            new(new DateTime(2021, 1, 1), 1.0, 2.0, 3.0, 4.0, 5.0),
            new(new DateTime(2021, 1, 2), 1.1, 2.1, 3.1, 4.1, 5.1)
        ];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeTrue();
        result.Mismatches.Should().BeEmpty();
    }

    [TestMethod]
    public void MultiPropertyIndicator_OneMismatch_ReturnsMismatch()
    {
        // Arrange
        List<MacdResult> expected =
        [
            new(new DateTime(2021, 1, 1), 1.0, 2.0, 3.0, 4.0, 5.0)
        ];

        List<MacdResult> actual =
        [
            new(new DateTime(2021, 1, 1), 1.0, 2.1, 3.0, 4.0, 5.0) // Signal differs
        ];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeFalse();
        result.Mismatches.Should().HaveCount(1);

        MismatchDetail mismatch = result.Mismatches[0];
        mismatch.PropertyName.Should().Be("Signal");
        mismatch.Expected.Should().Be(2.0);
        mismatch.Actual.Should().Be(2.1);
    }

    [TestMethod]
    public void EmptySequences_ReturnsMatch()
    {
        // Arrange
        List<SmaResult> expected = [];
        List<SmaResult> actual = [];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeTrue();
        result.Mismatches.Should().BeEmpty();
    }

    [TestMethod]
    public void SingleElement_Match_ReturnsMatch()
    {
        // Arrange
        List<SmaResult> expected = [new(new DateTime(2021, 1, 1), 100.0)];
        List<SmaResult> actual = [new(new DateTime(2021, 1, 1), 100.0)];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeTrue();
        result.Mismatches.Should().BeEmpty();
    }

    [TestMethod]
    public void ExtremeValues_Match_ReturnsMatch()
    {
        // Arrange
        List<SmaResult> expected =
        [
            new(new DateTime(2021, 1, 1), double.MaxValue),
            new(new DateTime(2021, 1, 2), double.MinValue),
            new(new DateTime(2021, 1, 3), double.Epsilon)
        ];

        List<SmaResult> actual =
        [
            new(new DateTime(2021, 1, 1), double.MaxValue),
            new(new DateTime(2021, 1, 2), double.MinValue),
            new(new DateTime(2021, 1, 3), double.Epsilon)
        ];

        // Act
        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        // Assert
        result.IsMatch.Should().BeTrue();
        result.Mismatches.Should().BeEmpty();
    }
}
