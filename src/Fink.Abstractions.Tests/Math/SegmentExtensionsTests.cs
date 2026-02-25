namespace Fink.Abstractions.Tests.Math;

public class SegmentExtensionsTests
{
    [Fact]
    public void HasLowerBound_WhenMinHasValue_ReturnsTrue()
    {
        var segment = new Segment(true, true, 5u, 10u);

        var result = segment.HasLowerBound();

        Assert.True(result);
    }

    [Fact]
    public void HasLowerBound_WhenMinIsNull_ReturnsFalse()
    {
        var segment = new Segment(true, true, null, 10u);

        var result = segment.HasLowerBound();

        Assert.False(result);
    }

    [Fact]
    public void HasLowerBound_WhenMinIsZero_ReturnsTrue()
    {
        var segment = new Segment(true, true, 0u, 10u);

        var result = segment.HasLowerBound();

        Assert.True(result);
    }

    [Fact]
    public void HasUpperBound_WhenMaxHasValue_ReturnsTrue()
    {
        var segment = new Segment(true, true, 5u, 10u);

        var result = segment.HasUpperBound();

        Assert.True(result);
    }

    [Fact]
    public void HasUpperBound_WhenMaxIsNull_ReturnsFalse()
    {
        var segment = new Segment(true, true, 5u, null);

        var result = segment.HasUpperBound();

        Assert.False(result);
    }

    [Fact]
    public void HasUpperBound_WhenMaxIsMaxValue_ReturnsTrue()
    {
        var segment = new Segment(true, true, 5u, uint.MaxValue);

        var result = segment.HasUpperBound();

        Assert.True(result);
    }

    [Fact]
    public void HasLowerBound_AndHasUpperBound_WhenBothNull_ReturnsFalse()
    {
        var segment = new Segment(true, true, null, null);

        var hasLower = segment.HasLowerBound();
        var hasUpper = segment.HasUpperBound();

        Assert.False(hasLower);
        Assert.False(hasUpper);
    }

    [Fact]
    public void HasLowerBound_AndHasUpperBound_WhenBothHaveValues_ReturnsTrue()
    {
        var segment = new Segment(false, false, 1u, 100u);

        var hasLower = segment.HasLowerBound();
        var hasUpper = segment.HasUpperBound();

        Assert.True(hasLower);
        Assert.True(hasUpper);
    }
}