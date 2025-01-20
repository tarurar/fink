using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Fink.Abstractions.Tests;


public class FilePathTests
{
    [Fact]
    public void FilePath_When_Empty_Value_Passed_In_Creates_Empty_Path()
    {
        FilePath filePath = new(string.Empty);

        Assert.True(filePath.IsEmpty);
    }

    [Fact]
    public void FilePath_Empty_Creates_With_Empty_Path()
    {
        FilePath emptyPath = FilePath.Empty;

        Assert.Equal(string.Empty, emptyPath);
    }

    [Theory]
    [InlineData('\0')]
    public void FilePath_When_InvalidPathChars_Used_RaisesException(char invalidChar)
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new FilePath(new string(invalidChar, 10)));

        Assert.Contains("invalid", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Theory]
    [InlineData("12345", "12345")]
    [InlineData(" 12345", "12345")]
    [InlineData("12345 ", "12345")]
    [InlineData("""
        12345     
    """, "12345")]
    public void FilePath_Trims_Value(string source, string expected)
    {
        FilePath filePath = new(source);

        Assert.Equal(expected, filePath);
    }

    [Fact]
    public void ImplicitConversion_ToString_Returns_Raw_String_Value()
    {
        Prop.ForAll(Gen.PathString, pathString =>
        {
            FilePath filePath = new(pathString);
            return filePath.Value == filePath;
        }).QuickCheckThrowOnFailure();
    }
}