namespace Fink.Abstractions.Tests;

public class FilePathExtensionsTests(TemporaryFileFixture fixture) : IClassFixture<TemporaryFileFixture>
{
    private readonly TemporaryFileFixture _fixture = fixture;

    [Fact]
    public void AssertFilePathExists_FileExists_ReturnsFilePath()
    {
        FilePath result = _fixture.FilePath.AssertFilePathExists();

        Assert.Equal(_fixture.FilePath, result);
    }

    [Fact]
    public void AssertFilePathExists_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        FilePath filePath = "non-existent-file.txt";

        FileNotFoundException ex = Assert.Throws<FileNotFoundException>(filePath.AssertFilePathExists);
        Assert.Contains(filePath, ex.Message, StringComparison.InvariantCulture);
    }

    [Fact]
    public void AssertFilePathHasExtension_FileHasExtension_ReturnsFilePath()
    {
        FilePath filePath = Path.Combine(Path.GetTempPath(), "file.txt");

        FilePath result = filePath.AssertFilePathHasExtension(".txt");

        Assert.Equal(filePath, result);
    }

    [Fact]
    public void AssertFilePathHasExtension_FileHasAnotherExtension_ThrowsArgumentException()
    {
        FilePath filePath = Path.Combine(Path.GetTempPath(), "file.txt");

        ArgumentException ex = Assert.Throws<ArgumentException>(() => filePath.AssertFilePathHasExtension(".pdf"));
        Assert.Contains(filePath, ex.Message, StringComparison.InvariantCulture);
    }

    [Fact]
    public void AssertFilePathHasExtension_FileWithoutExtension_ThrowsArgumentException()
    {
        FilePath filePath = Path.Combine(Path.GetTempPath(), "file");

        ArgumentException ex = Assert.Throws<ArgumentException>(() => filePath.AssertFilePathHasExtension(".txt"));
        Assert.Contains(expectedSubstring: filePath, ex.Message, StringComparison.InvariantCulture);
    }

    [Fact]
    public void AssertFilePathHasExtension_FileWithoutExtension_EmptyExtension_ReturnsFilePath()
    {
        FilePath filePath = Path.Combine(Path.GetTempPath(), "file");

        FilePath result = filePath.AssertFilePathHasExtension(string.Empty);

        Assert.Equal(filePath, result);
    }

    [Fact]
    public void AssertFilePathHasExtension_IgnoresExtensionCase_ReturnsFilePath()
    {
        FilePath filePath = Path.Combine(Path.GetTempPath(), "file.txt");

        FilePath result = filePath.AssertFilePathHasExtension(".TXT");

        Assert.Equal(filePath, result);
    }
}