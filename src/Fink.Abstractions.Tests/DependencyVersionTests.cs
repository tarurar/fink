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

    [Fact]
    public void Constructor_Trims_Version()
    {
        DependencyVersion version = new(" 1.0.0 ");

        string actual = version;

        Assert.Equal("1.0.0", actual);
    }
}