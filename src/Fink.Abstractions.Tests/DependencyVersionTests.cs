namespace Fink.Abstractions.Tests;

public class DependencyVersionTests
{
    [Fact]
    public void Constructor_NullVersion_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new DependencyVersion(null!));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_EmptyVersion_ThrowsArgumentException(string version) =>
        Assert.Throws<ArgumentException>(() => new DependencyVersion(version));

    [Fact]
    public void ImplicitConversion_ValidVersion_Returns_RawVersion()
    {
        DependencyVersion version = new("1.0.0");

        string actual = version;

        Assert.Equal("1.0.0", actual);
    }

    [Fact]
    public void ToString_ValidVersion_Returns_RawVersion()
    {
        DependencyVersion version = new("1.0.0");

        string actual = version.ToString();

        Assert.Equal("1.0.0", actual);
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.1", false)]
    [InlineData("1.0.0", "2.0.0", false)]
    [InlineData("1.0.0-alpha", "1.0.0-alpha", true)]
    public void Equals_VersionComparison_ReturnsExpected(string version1, string version2, bool expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expected, v1.Equals(v2));
        Assert.Equal(expected, v1.Equals((object)v2));
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.1", false)]
    [InlineData("1.0.0", "2.0.0", false)]
    public void EqualityOperator_VersionComparison_ReturnsExpected(string version1, string version2, bool expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expected, v1 == v2);
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0", false)]
    [InlineData("1.0.0", "1.0.1", true)]
    [InlineData("1.0.0", "2.0.0", true)]
    public void InequalityOperator_VersionComparison_ReturnsExpected(string version1, string version2, bool expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expected, v1 != v2);
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0", 0)]
    [InlineData("1.0.0", "1.0.1", -1)]
    [InlineData("1.0.1", "1.0.0", 1)]
    [InlineData("1.0.0", "2.0.0", -1)]
    [InlineData("2.0.0", "1.0.0", 1)]
    [InlineData("1.0.0-alpha", "1.0.0-beta", -1)]
    [InlineData("1.0.0-beta", "1.0.0-alpha", 1)]
    public void CompareTo_VersionComparison_ReturnsExpected(string version1, string version2, int expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        var result = v1.CompareTo(v2);
        Assert.Equal(Math.Sign(expected), Math.Sign(result));
    }

    [Theory]
    [InlineData("1.0.0", "1.0.1", true)]
    [InlineData("1.0.0", "2.0.0", true)]
    [InlineData("1.0.1", "1.0.0", false)]
    [InlineData("1.0.0", "1.0.0", false)]
    public void LessThanOperator_VersionComparison_ReturnsExpected(string version1, string version2, bool expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expected, v1 < v2);
    }

    [Theory]
    [InlineData("1.0.1", "1.0.0", true)]
    [InlineData("2.0.0", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.1", false)]
    [InlineData("1.0.0", "1.0.0", false)]
    public void GreaterThanOperator_VersionComparison_ReturnsExpected(string version1, string version2, bool expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expected, v1 > v2);
    }

    [Theory]
    [InlineData("1.0.0", "1.0.1", true)]
    [InlineData("1.0.0", "2.0.0", true)]
    [InlineData("1.0.0", "1.0.0", true)]
    [InlineData("1.0.1", "1.0.0", false)]
    public void LessThanOrEqualOperator_VersionComparison_ReturnsExpected(string version1, string version2, bool expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expected, v1 <= v2);
    }

    [Theory]
    [InlineData("1.0.1", "1.0.0", true)]
    [InlineData("2.0.0", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.1", false)]
    public void GreaterThanOrEqualOperator_VersionComparison_ReturnsExpected(string version1, string version2, bool expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expected, v1 >= v2);
    }

    [Fact]
    public void CompareTo_NullObject_ReturnsPositive()
    {
        var version = new DependencyVersion("1.0.0");

        var result = version.CompareTo(null);

        Assert.True(result > 0);
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var version = new DependencyVersion("1.0.0");

        Assert.False(version.Equals("1.0.0"));
        Assert.False(version.Equals(42));
    }

    [Theory]
    [InlineData("1.0", "1.0.0")]
    [InlineData("1", "1.0")]
    [InlineData("1.0.0-alpha", "1.0-alpha")]
    public void VersionComparison_DifferentFormats_WorksCorrectly(string version1, string version2)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(0, v1.CompareTo(v2));
    }

    [Theory]
    [InlineData("v1.0.0", "1.0.0", true)]
    [InlineData("v1.0.0", "v1.0.0", true)]
    [InlineData("v1.0.0", "v1.0.1", false)]
    [InlineData("v1.0.0-alpha", "v1.0.0-alpha", true)]
    public void Equals_VersionWithVPrefix_ReturnsExpected(string version1, string version2, bool expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expected, v1.Equals(v2));
        Assert.Equal(expected, v1.Equals((object)v2));
    }

    [Theory]
    [InlineData("v1.0.0", "v1.0.1", -1)]
    [InlineData("v1.0.1", "v1.0.0", 1)]
    [InlineData("v2.0.0", "v1.0.0", 1)]
    [InlineData("v1.0.0-alpha", "v1.0.0-beta", -1)]
    [InlineData("v1.0.0-beta", "v1.0.0-alpha", 1)]
    [InlineData("v1.0.0", "1.0.0", 0)]
    public void CompareTo_VersionWithVPrefix_ReturnsExpected(string version1, string version2, int expected)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        var result = v1.CompareTo(v2);
        Assert.Equal(Math.Sign(expected), Math.Sign(result));
    }

    [Theory]
    [InlineData("v1.0.0", "v1.0.1", true)]
    [InlineData("v1.0.0", "v2.0.0", true)]
    [InlineData("v1.0.1", "v1.0.0", false)]
    [InlineData("v1.0.0", "v1.0.0", false)]
    [InlineData("v1.0.0", "1.0.1", true)]
    [InlineData("v1.0.0", "2.0.0", true)]
    [InlineData("v1.0.0-alpha", "v1.0.0-beta", true)]
    [InlineData("v2.0.0-alpha", "v1.0.0-beta", false)]
    public void ComparisonOperators_VersionWithVPrefix_ReturnsExpected(string version1, string version2, bool expectedLessThan)
    {
        var v1 = new DependencyVersion(version1);
        var v2 = new DependencyVersion(version2);

        Assert.Equal(expectedLessThan, v1 < v2);
        Assert.Equal(!expectedLessThan && v1.CompareTo(v2) != 0, v1 > v2);
        Assert.Equal(expectedLessThan || v1.CompareTo(v2) == 0, v1 <= v2);
        Assert.Equal(!expectedLessThan, v1 >= v2);
    }
}