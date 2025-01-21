namespace Fink.Abstractions.Tests;

public class DependencyNameTests
{
    [Fact]
    public void ImplicitConversionToString_Returns_RawDependencyName()
    {
        string dependencyName = "dependency-name";

        DependencyName actual = new(dependencyName);

        Assert.Equal(dependencyName, actual);
    }

    [Fact]
    public void ToString_Returns_RawDependencyName()
    {
        string dependencyName = "dependency-name";

        DependencyName actual = new(dependencyName);

        Assert.Equal(dependencyName, actual.ToString());
    }
}