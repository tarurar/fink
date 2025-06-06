namespace Fink.Abstractions.Tests;

public class DependencyTests
{
    [Fact]
    public void When_ParentDependencyIsNull_Then_PathIsCreatedWithSingleSegment()
    {
        Dependency dependency = new(new("dependency"));

        DependencyPathSegment rootSegment = Assert.Single(dependency.Path.Segments);
        Assert.Equal("dependency", rootSegment.ToString());
    }

    [Fact]
    public void When_ParentDependencyProvided_Then_PathIsCreatedWithMultipleSegments()
    {
        Dependency parentDependency = new(new("parent"));
        Dependency dependency = new(new("dependency"), ParentDependency: parentDependency);

        Assert.Equal(2, dependency.Path.Segments.Count);
    }

    [Fact]
    public void When_VersionIsNull_Then_PathSegmentIsCreatedWithoutVersion()
    {
        Dependency dependency = new(new("dependency"));

        DependencyPathSegment rootSegment = Assert.Single(dependency.Path.Segments);
        Assert.Equal("dependency", rootSegment.ToString());
    }

    [Fact]
    public void When_VersionIsProvided_Then_PathSegmentIsCreatedWithVersion()
    {
        Dependency dependency = new(new("dependency"), new("1.0.0"));

        DependencyPathSegment rootSegment = Assert.Single(dependency.Path.Segments);
        Assert.Equal("dependency 1.0.0", rootSegment.ToString());
    }
}