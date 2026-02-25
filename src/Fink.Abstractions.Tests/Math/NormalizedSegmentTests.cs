namespace Fink.Abstractions.Tests.Math;

public class NormalizedSegmentTests
{
    [Theory]
    [InlineData(5u, 10u)]
    [InlineData(0u, 100u)]
    [InlineData(1u, 1u)]
    [InlineData(0u, uint.MaxValue)]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly(uint min, uint max)
    {
        var normalizedSegment = new NormalizedSegment(min, max);

        Assert.Equal(min, normalizedSegment.Min);
        Assert.Equal(max, normalizedSegment.Max);
    }

    [Fact]
    public void FromSegment_WithInclusiveBounds_ReturnsExactValues()
    {
        var segment = new Segment(true, true, 5u, 10u);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(5u, normalized.Min);
        Assert.Equal(10u, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithExclusiveBounds_AdjustsValues()
    {
        var segment = new Segment(false, false, 5u, 10u);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(6u, normalized.Min);
        Assert.Equal(9u, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithMixedBounds_AdjustsCorrectly()
    {
        var segment = new Segment(true, false, 5u, 10u);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(5u, normalized.Min);
        Assert.Equal(9u, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithMixedBoundsReverse_AdjustsCorrectly()
    {
        var segment = new Segment(false, true, 5u, 10u);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(6u, normalized.Min);
        Assert.Equal(10u, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithNullMin_UsesZeroDefault()
    {
        var segment = new Segment(true, true, null, 10u);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(0u, normalized.Min);
        Assert.Equal(10u, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithNullMax_UsesMaxValueDefault()
    {
        var segment = new Segment(true, true, 5u, null);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(5u, normalized.Min);
        Assert.Equal(uint.MaxValue, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithBothNullValues_UsesDefaults()
    {
        var segment = new Segment(true, true, null, null);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(0u, normalized.Min);
        Assert.Equal(uint.MaxValue, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithNullMinExclusive_AdjustsFromZero()
    {
        var segment = new Segment(false, true, null, 10u);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(1u, normalized.Min);
        Assert.Equal(10u, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithNullMaxExclusive_AdjustsFromMaxValue()
    {
        var segment = new Segment(true, false, 5u, null);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(5u, normalized.Min);
        Assert.Equal(uint.MaxValue - 1, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithBothNullExclusive_AdjustsBothDefaults()
    {
        var segment = new Segment(false, false, null, null);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(1u, normalized.Min);
        Assert.Equal(uint.MaxValue - 1, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithZeroExclusive_AdjustsToOne()
    {
        var segment = new Segment(false, true, 0u, 10u);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(1u, normalized.Min);
        Assert.Equal(10u, normalized.Max);
    }

    [Fact]
    public void FromSegment_WithMaxValueExclusive_AdjustsToMaxMinusOne()
    {
        var segment = new Segment(true, false, 5u, uint.MaxValue);

        var normalized = NormalizedSegment.FromSegment(segment);

        Assert.Equal(5u, normalized.Min);
        Assert.Equal(uint.MaxValue - 1, normalized.Max);
    }
}