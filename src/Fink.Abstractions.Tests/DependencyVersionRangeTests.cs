using NuGet.Versioning;

namespace Fink.Abstractions.Tests;

public class DependencyVersionRangeTests
{
    [Fact]
    public void Constructor_NullVersionRange_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new DependencyVersionRange((string)null!));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_EmptyVersionRange_ThrowsArgumentException(string versionRange) =>
        Assert.Throws<ArgumentException>(() => new DependencyVersionRange(versionRange));


    [Fact]
    public void ToString_ValidVersionRange_Returns_RawVersionRange()
    {
        DependencyVersionRange versionRange = new("[1.0.0, 2.0.0)");

        string actual = versionRange.ToString();

        Assert.Equal("[1.0.0, 2.0.0)", actual);
    }

    [Theory]
    [InlineData("[1.0.0, 2.0.0)", "[1.0.0, 2.0.0)", true)]
    [InlineData("[1.0.0, 2.0.0)", "[1.0.0, 3.0.0)", false)]
    [InlineData("(1.0.0, 2.0.0]", "(1.0.0, 2.0.0]", true)]
    [InlineData("1.0.0", "1.0.0", true)]
    public void Equals_VersionRangeComparison_ReturnsExpected(string versionRange1, string versionRange2, bool expected)
    {
        var vr1 = new DependencyVersionRange(versionRange1);
        var vr2 = new DependencyVersionRange(versionRange2);

        Assert.Equal(expected, vr1.Equals(vr2));
        Assert.Equal(expected, vr1.Equals((object)vr2));
    }

    [Theory]
    [InlineData("[1.0.0, 2.0.0)", "[1.0.0, 2.0.0)", true)]
    [InlineData("[1.0.0, 2.0.0)", "[1.0.0, 3.0.0)", false)]
    [InlineData("1.0.0", "1.0.0", true)]
    public void EqualityOperator_VersionRangeComparison_ReturnsExpected(string versionRange1, string versionRange2, bool expected)
    {
        var vr1 = new DependencyVersionRange(versionRange1);
        var vr2 = new DependencyVersionRange(versionRange2);

        Assert.Equal(expected, vr1 == vr2);
    }

    [Theory]
    [InlineData("[1.0.0, 2.0.0)", "[1.0.0, 2.0.0)", false)]
    [InlineData("[1.0.0, 2.0.0)", "[1.0.0, 3.0.0)", true)]
    [InlineData("1.0.0", "2.0.0", true)]
    public void InequalityOperator_VersionRangeComparison_ReturnsExpected(string versionRange1, string versionRange2, bool expected)
    {
        var vr1 = new DependencyVersionRange(versionRange1);
        var vr2 = new DependencyVersionRange(versionRange2);

        Assert.Equal(expected, vr1 != vr2);
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var versionRange = new DependencyVersionRange("[1.0.0, 2.0.0)");

        Assert.False(versionRange.Equals("[1.0.0, 2.0.0)"));
        Assert.False(versionRange.Equals(42));
    }

    [Theory]
    [InlineData("[1.0.0,)", "Minimum version")]
    [InlineData("(,2.0.0)", "Maximum version")]
    [InlineData("[1.0.0, 2.0.0)", "Range with inclusive minimum, exclusive maximum")]
    [InlineData("(1.0.0, 2.0.0]", "Range with exclusive minimum, inclusive maximum")]
    [InlineData("[1.0.0, 2.0.0]", "Range with inclusive minimum and maximum")]
    [InlineData("(1.0.0, 2.0.0)", "Range with exclusive minimum and maximum")]
    public void VersionRangeComparison_DifferentFormats_WorksCorrectly(string versionRange, string description)
    {
        var vr = new DependencyVersionRange(versionRange);

        // This test verifies that different valid formats are handled without throwing exceptions
        Assert.NotNull(vr);
        Assert.NotEmpty(vr.ToString());
        Assert.NotEmpty(description); // Use the description parameter
    }

    [Theory]
    [InlineData("1.0.0")]
    [InlineData("[1.0.0,)")]
    [InlineData("(,2.0.0)")]
    [InlineData("[1.0.0, 2.0.0)")]
    [InlineData("(1.0.0, 2.0.0]")]
    [InlineData("[1.0.0, 2.0.0]")]
    [InlineData("(1.0.0, 2.0.0)")]
    public void Constructor_ValidVersionRangeFormats_DoesNotThrow(string versionRange)
    {
        var exception = Record.Exception(() => new DependencyVersionRange(versionRange));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("[1.0.0")]
    [InlineData("1.0.0]")]
    [InlineData("[1.0.0, 2.0.0")]
    [InlineData("1.0.0, 2.0.0)")]
    public void Constructor_InvalidVersionRangeFormats_ThrowsArgumentException(string invalidVersionRange) =>
        _ = Assert.Throws<ArgumentException>(() => new DependencyVersionRange(invalidVersionRange));

    [Fact]
    public void Constructor_WithVersionRange_CreatesCorrectInstance()
    {
        var nugetVersionRange = VersionRange.Parse("[1.2.3, 4.5.6)");
        var dependencyVersionRange = new DependencyVersionRange(nugetVersionRange);

        Assert.Equal("[1.2.3, 4.5.6)", dependencyVersionRange.ToString());
    }

    [Theory]
    [InlineData("[1.0.0,)", "1.0.0")]
    [InlineData("[2.1.3, 4.0.0)", "2.1.3")]
    [InlineData("(1.0.0, 2.0.0]", "1.0.0")]
    [InlineData("(,2.0.0)", "0.0.0")]
    public void MinVersion_Property_ReturnsExpectedVersion(string rangeString, string expectedMinVersion)
    {
        var versionRange = new DependencyVersionRange(rangeString);
        
        Assert.Equal(expectedMinVersion, versionRange.MinVersion.ToString());
    }

    [Theory]
    [InlineData("[1.0.0,)", "[1.0.0, )")]
    [InlineData("1.0.0", "[1.0.0, )")]
    [InlineData("[1.0.0, 2.0.0)", "[1.0.0, 2.0.0)")]
    [InlineData("(1.0.0, 2.0.0]", "(1.0.0, 2.0.0]")]
    public void Constructor_StringAndVersionRange_ProduceSameResult(string versionRangeString, string expectedString)
    {
        var fromString = new DependencyVersionRange(versionRangeString);
        var fromVersionRange = new DependencyVersionRange(VersionRange.Parse(versionRangeString));

        Assert.Equal(expectedString, fromString.ToString());
        Assert.Equal(fromString.ToString(), fromVersionRange.ToString());
        Assert.Equal(fromString, fromVersionRange);
    }

    [Fact]
    public void GetHashCode_SameVersionRanges_ReturnsSameHashCode()
    {
        var versionRange1 = new DependencyVersionRange("[1.0.0, 2.0.0)");
        var versionRange2 = new DependencyVersionRange("[1.0.0, 2.0.0)");
        
        Assert.Equal(versionRange1.GetHashCode(), versionRange2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentVersionRanges_ReturnsDifferentHashCodes()
    {
        var versionRange1 = new DependencyVersionRange("[1.0.0, 2.0.0)");
        var versionRange2 = new DependencyVersionRange("[1.0.0, 3.0.0)");
        
        Assert.NotEqual(versionRange1.GetHashCode(), versionRange2.GetHashCode());
    }
}
