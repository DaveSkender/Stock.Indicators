namespace StreamHubs;

[TestClass]
public class RollingWindowTests : TestBase
{
    [TestMethod]
    public void RollingWindowMax_WithNaN_ReturnsNaN()
    {
        // Arrange
        RollingWindowMax<double> window = new(3);

        // Act: Add normal values, then NaN
        window.Add(10.0);
        window.Add(20.0);
        window.Add(double.NaN);

        // Assert: Max should return NaN when NaN is in window
        double max = window.GetMax();
        max.Should().Be(double.NaN);
    }

    [TestMethod]
    public void RollingWindowMax_NaNEvicted_ReturnsNormalMax()
    {
        // Arrange
        RollingWindowMax<double> window = new(3);

        // Act: Add NaN, then normal values to evict it
        window.Add(double.NaN);
        window.Add(10.0);
        window.Add(20.0);
        window.Add(30.0); // This evicts the NaN

        // Assert: Max should return normal value after NaN is evicted
        double max = window.GetMax();
        max.Should().Be(30.0);
    }

    [TestMethod]
    public void RollingWindowMin_WithNaN_ReturnsNaN()
    {
        // Arrange
        RollingWindowMin<double> window = new(3);

        // Act: Add normal values, then NaN
        window.Add(10.0);
        window.Add(20.0);
        window.Add(double.NaN);

        // Assert: Min should return NaN when NaN is in window
        double min = window.GetMin();
        min.Should().Be(double.NaN);
    }

    [TestMethod]
    public void RollingWindowMin_NaNEvicted_ReturnsNormalMin()
    {
        // Arrange
        RollingWindowMin<double> window = new(3);

        // Act: Add NaN, then normal values to evict it
        window.Add(double.NaN);
        window.Add(10.0);
        window.Add(20.0);
        window.Add(30.0); // This evicts the NaN

        // Assert: Min should return normal value after NaN is evicted
        double min = window.GetMin();
        min.Should().Be(10.0);
    }

    [TestMethod]
    public void RollingWindowMax_MultipleNaN_ReturnsNaN()
    {
        // Arrange
        RollingWindowMax<double> window = new(5);

        // Act: Add multiple NaN values
        window.Add(10.0);
        window.Add(double.NaN);
        window.Add(20.0);
        window.Add(double.NaN);
        window.Add(30.0);

        // Assert: Max should return NaN when multiple NaN values are in window
        double max = window.GetMax();
        max.Should().Be(double.NaN);
    }

    [TestMethod]
    public void RollingWindowMin_AllNaN_Evicted_ReturnsNormalMin()
    {
        // Arrange
        RollingWindowMin<double> window = new(2);

        // Act: Fill with NaN, then replace with normal values
        window.Add(double.NaN);
        window.Add(double.NaN);
        window.Add(5.0);  // Evicts first NaN
        window.Add(3.0);  // Evicts second NaN

        // Assert: Min should work normally after all NaN values evicted
        double min = window.GetMin();
        min.Should().Be(3.0);
    }

    [TestMethod]
    public void RollingWindowMax_Clear_ResetsNaNTracking()
    {
        // Arrange
        RollingWindowMax<double> window = new(3);
        window.Add(double.NaN);
        window.Add(10.0);

        // Act: Clear and add normal values
        window.Clear();
        window.Add(5.0);
        window.Add(15.0);

        // Assert: Max should work normally after clear
        double max = window.GetMax();
        max.Should().Be(15.0);
    }
}
