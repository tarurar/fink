namespace Fink.Abstractions.Tests;

public class DependencyTests
{
    [Fact]
    public void When_ParentDependencyIsNull_Then_PathIsCreatedWithSingleSegment()
    {
        Dependency dependency = new(new("dependency"), new DependencyVersion("1.0"));

        DependencyPathSegment rootSegment = Assert.Single(dependency.Path.Segments);
        Assert.Equal("dependency 1.0", rootSegment.ToString());
    }

    [Fact]
    public void When_ParentDependencyProvided_Then_PathIsCreatedWithMultipleSegments()
    {
        Dependency parentDependency = new(new("parent"), new DependencyVersion("1.0"));
        Dependency dependency = new(new("dependency"), new DependencyVersion("2.0"), ParentDependency: parentDependency);

        Assert.Equal(2, dependency.Path.Segments.Count);
    }
}