internal record Segment
{
    public bool IsMinInclusive { get; init; }
    public bool IsMaxInclusive { get; init; }
    public uint? Min { get; init; }
    public uint? Max { get; init; }

    public Segment(bool isMinInclusive, bool isMaxInclusive, uint? min, uint? max)
    {
        if (min > max)
        {
            throw new ArgumentException("Min cannot be greater than Max.");
        }

        IsMinInclusive = isMinInclusive;
        IsMaxInclusive = isMaxInclusive;
        Min = min;
        Max = max;
    }
}

internal record NormalizedSegment(uint Min, uint Max)
{
    public static NormalizedSegment FromSegment(Segment segment)
    {
        var min = segment.IsMinInclusive
            ? segment.Min ?? 0
            : (segment.Min ?? 0) + 1;

        var max = segment.IsMaxInclusive
            ? segment.Max ?? uint.MaxValue
            : (segment.Max ?? uint.MaxValue) - 1;

        return new NormalizedSegment(min, max);
    }
}

internal static class SegmentExtensions
{
    public static bool HasLowerBound(this Segment segment) => segment.Min.HasValue;
    public static bool HasUpperBound(this Segment segment) => segment.Max.HasValue;
}