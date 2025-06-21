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

    [Fact]
    public void Path_WithDeepHierarchy_CreatesCorrectPath()
    {
        var grandParent = new Dependency(new DependencyName("grandparent"), new DependencyVersion("1.0.0"));
        var parent = new Dependency(new DependencyName("parent"), new DependencyVersion("2.0.0"), grandParent);
        var child = new Dependency(new DependencyName("child"), new DependencyVersion("3.0.0"), parent);

        Assert.Equal(3, child.Path.Segments.Count);
        Assert.Equal("grandparent 1.0.0", child.Path.Segments[0].ToString());
        Assert.Equal("parent 2.0.0", child.Path.Segments[1].ToString());
        Assert.Equal("child 3.0.0", child.Path.Segments[2].ToString());
    }

    [Fact]
    public void Equals_SameDependencies_ReturnsTrue()
    {
        var dependency1 = new Dependency(new DependencyName("test"), new DependencyVersion("1.0.0"));
        var dependency2 = new Dependency(new DependencyName("test"), new DependencyVersion("1.0.0"));

        Assert.Equal(dependency1, dependency2);
    }

    [Fact]
    public void Equals_DifferentNames_ReturnsFalse()
    {
        var dependency1 = new Dependency(new DependencyName("test1"), new DependencyVersion("1.0.0"));
        var dependency2 = new Dependency(new DependencyName("test2"), new DependencyVersion("1.0.0"));

        Assert.NotEqual(dependency1, dependency2);
    }

    [Fact]
    public void Equals_DifferentVersions_ReturnsFalse()
    {
        var dependency1 = new Dependency(new DependencyName("test"), new DependencyVersion("1.0.0"));
        var dependency2 = new Dependency(new DependencyName("test"), new DependencyVersion("2.0.0"));

        Assert.NotEqual(dependency1, dependency2);
    }

    [Fact]
    public void Equals_DifferentParents_ReturnsFalse()
    {
        var parent1 = new Dependency(new DependencyName("parent1"), new DependencyVersion("1.0.0"));
        var parent2 = new Dependency(new DependencyName("parent2"), new DependencyVersion("1.0.0"));
        var dependency1 = new Dependency(new DependencyName("test"), new DependencyVersion("1.0.0"), parent1);
        var dependency2 = new Dependency(new DependencyName("test"), new DependencyVersion("1.0.0"), parent2);

        Assert.NotEqual(dependency1, dependency2);
    }

    [Fact]
    public void GetHashCode_SameDependencies_ReturnsSameHashCode()
    {
        var dependency1 = new Dependency(new DependencyName("test"), new DependencyVersion("1.0.0"));
        var dependency2 = new Dependency(new DependencyName("test"), new DependencyVersion("1.0.0"));

        Assert.Equal(dependency1.GetHashCode(), dependency2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentDependencies_ReturnsDifferentHashCodes()
    {
        var dependency1 = new Dependency(new DependencyName("test1"), new DependencyVersion("1.0.0"));
        var dependency2 = new Dependency(new DependencyName("test2"), new DependencyVersion("1.0.0"));

        Assert.NotEqual(dependency1.GetHashCode(), dependency2.GetHashCode());
    }

    [Fact]
    public void ToString_DefaultRecord_ReturnsExpectedFormat()
    {
        var dependency = new Dependency(new DependencyName("test-package"), new DependencyVersion("1.2.3"));

        var result = dependency.ToString();

        Assert.Contains("test-package", result, StringComparison.Ordinal);
        Assert.Contains("1.2.3", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Name_Property_ReturnsCorrectName()
    {
        var dependency = new Dependency(new DependencyName("test-package"), new DependencyVersion("1.0.0"));

        Assert.Equal("test-package", dependency.Name.Name);
    }

    [Fact]
    public void Versioning_Property_ReturnsCorrectVersioning()
    {
        var version = new DependencyVersion("1.2.3");
        var dependency = new Dependency(new DependencyName("test-package"), version);

        Assert.Equal(version, dependency.Versioning);
    }

    [Fact]
    public void ParentDependency_Property_ReturnsCorrectParent()
    {
        var parent = new Dependency(new DependencyName("parent"), new DependencyVersion("1.0.0"));
        var child = new Dependency(new DependencyName("child"), new DependencyVersion("2.0.0"), parent);

        Assert.Equal(parent, child.ParentDependency);
    }

    [Fact]
    public void ParentDependency_WithNullParent_ReturnsNull()
    {
        var dependency = new Dependency(new DependencyName("test"), new DependencyVersion("1.0.0"));

        Assert.Null(dependency.ParentDependency);
    }

    [Theory]
    [InlineData("package-name")]
    [InlineData("My.Package")]
    [InlineData("some_package")]
    [InlineData("package123")]
    public void Constructor_WithValidNames_CreatesValidDependency(string packageName)
    {
        var dependency = new Dependency(new DependencyName(packageName), new DependencyVersion("1.0.0"));

        Assert.Equal(packageName, dependency.Name.Name);
        Assert.Equal("1.0.0", dependency.Versioning.ToString());
    }
}