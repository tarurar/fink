using NuGet.Versioning;

namespace Fink.Abstractions.Tests;

public class DependencyVersionTests
{
    [Fact]
    public void Constructor_NullStringVersion_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new DependencyVersion((string)null!));


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_EmptyVersion_ThrowsArgumentException(string version) =>
        Assert.Throws<ArgumentException>(() => new DependencyVersion(version));


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

    [Fact]
    public void Constructor_WithNuGetVersion_CreatesCorrectInstance()
    {
        var nugetVersion = NuGetVersion.Parse("1.2.3");
        var dependencyVersion = new DependencyVersion(nugetVersion);

        Assert.Equal("1.2.3", dependencyVersion.ToString());
    }

    [Fact]
    public void Constructor_WithNuGetVersion_PreservesPrereleaseInfo()
    {
        var nugetVersion = NuGetVersion.Parse("1.2.3-alpha.1");
        var dependencyVersion = new DependencyVersion(nugetVersion);

        Assert.Equal("1.2.3-alpha.1", dependencyVersion.ToString());
    }

    [Theory]
    [InlineData("1.0.0")]
    [InlineData("2.1.0-beta")]
    [InlineData("1.0.0-alpha.1+build.123")]
    public void Constructor_StringAndNuGetVersion_ProduceSameResult(string versionString)
    {
        var fromString = new DependencyVersion(versionString);
        var fromNuGetVersion = new DependencyVersion(NuGetVersion.Parse(versionString));

        Assert.Equal(fromString.ToString(), fromNuGetVersion.ToString());
        Assert.Equal(fromString, fromNuGetVersion);
    }

    [Fact]
    public void MinVersion_Property_ReturnsSelf()
    {
        var version = new DependencyVersion("1.2.3");

        Assert.Equal(version, version.MinVersion);
        Assert.Same(version, version.MinVersion);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("1.2.3.4.5")]
    [InlineData("not-a-version")]
    public void Constructor_InvalidVersionString_ThrowsArgumentException(string invalidVersion) =>
        Assert.Throws<ArgumentException>(() => new DependencyVersion(invalidVersion));

    [Fact]
    public void GetHashCode_SameVersions_ReturnsSameHashCode()
    {
        var version1 = new DependencyVersion("1.2.3");
        var version2 = new DependencyVersion("1.2.3");

        Assert.Equal(version1.GetHashCode(), version2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentVersions_ReturnsDifferentHashCodes()
    {
        var version1 = new DependencyVersion("1.2.3");
        var version2 = new DependencyVersion("1.2.4");

        Assert.NotEqual(version1.GetHashCode(), version2.GetHashCode());
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0")]
    [InlineData("1.2.3-alpha", "1.2.3-alpha")]
    [InlineData("1.0.0+build.1", "1.0.0")]
    public void Constructor_ValidVersionString_ParsesCorrectly(string input, string expectedOutput)
    {
        var version = new DependencyVersion(input);

        Assert.Equal(expectedOutput, version.ToString());
    }
}