namespace Fink.Abstractions.Tests;

public class DependencyPathTests
{
    [Fact]
    public void Create_CreatesPath_WithRootSegment()
    {
        DependencyPath path = DependencyPath.Create(new DependencyPathSegment("root"));

        DependencyPathSegment rootSegment = Assert.Single(path.Segments);
        Assert.Equal("root", rootSegment.Name);
    }

    [Fact]
    public void AddSegment_AddsSegment_ToTheEnd()
    {
        DependencyPath path = DependencyPath.Create(new DependencyPathSegment("root"))
            .AddSegment(new DependencyPathSegment("segment-1"))
            .AddSegment(new DependencyPathSegment("segment-2"))
            .AddSegment(new DependencyPathSegment("segment-3"));

        Assert.Equal("segment-3", path.Segments[^1].Name);
    }

    [Fact]
    public void ToString_ReturnsStringRepresentation_With_ArrowSeparator()
    {
        DependencyPath path = DependencyPath.Create(new DependencyPathSegment("root"))
            .AddSegment(new DependencyPathSegment("segment-1"))
            .AddSegment(new DependencyPathSegment("segment-2"))
            .AddSegment(new DependencyPathSegment("segment-3"));

        Assert.Equal("root->segment-1->segment-2->segment-3", path.ToString());
    }
}