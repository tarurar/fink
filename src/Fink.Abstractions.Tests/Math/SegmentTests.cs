namespace Fink.Abstractions.Tests.Math;

public class SegmentTests
{
    [Theory]
    [InlineData(true, true, 1u, 10u)]
    [InlineData(false, false, 0u, 100u)]
    [InlineData(true, false, 5u, 15u)]
    [InlineData(false, true, 2u, 8u)]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly(bool isMinInclusive, bool isMaxInclusive, uint min, uint max)
    {
        var segment = new Segment(isMinInclusive, isMaxInclusive, min, max);

        Assert.Equal(isMinInclusive, segment.IsMinInclusive);
        Assert.Equal(isMaxInclusive, segment.IsMaxInclusive);
        Assert.Equal(min, segment.Min);
        Assert.Equal(max, segment.Max);
    }

    [Theory]
    [InlineData(true, true, null, 10u)]
    [InlineData(false, false, 5u, null)]
    [InlineData(true, false, null, null)]
    public void Constructor_WithNullableParameters_SetsPropertiesCorrectly(bool isMinInclusive, bool isMaxInclusive, uint? min, uint? max)
    {
        var segment = new Segment(isMinInclusive, isMaxInclusive, min, max);

        Assert.Equal(isMinInclusive, segment.IsMinInclusive);
        Assert.Equal(isMaxInclusive, segment.IsMaxInclusive);
        Assert.Equal(min, segment.Min);
        Assert.Equal(max, segment.Max);
    }

    [Theory]
    [InlineData(10u, 5u)]
    [InlineData(100u, 50u)]
    [InlineData(1u, 0u)]
    public void Constructor_WhenMinGreaterThanMax_ThrowsArgumentException(uint min, uint max)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Segment(true, true, min, max));

        Assert.Equal("Min cannot be greater than Max.", exception.Message);
    }

    [Fact]
    public void Constructor_WhenMinEqualsMax_DoesNotThrow()
    {
        var segment = new Segment(true, true, 5u, 5u);

        Assert.Equal(5u, segment.Min);
        Assert.Equal(5u, segment.Max);
    }

    [Theory]
    [InlineData(null, 10u)]
    [InlineData(5u, null)]
    [InlineData(null, null)]
    public void Constructor_WithNullValues_DoesNotThrow(uint? min, uint? max)
    {
        var segment = new Segment(true, true, min, max);

        Assert.Equal(min, segment.Min);
        Assert.Equal(max, segment.Max);
    }

    [Fact]
    public void Constructor_WithBoundaryValues_HandlesCorrectly()
    {
        var segment = new Segment(true, true, 0u, uint.MaxValue);

        Assert.Equal(0u, segment.Min);
        Assert.Equal(uint.MaxValue, segment.Max);
    }
}