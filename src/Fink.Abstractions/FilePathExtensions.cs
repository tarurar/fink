namespace Fink.Abstractions;

public static class FilePathExtensions
{
    public static FilePath AssertFilePathExists(this FilePath filePath) =>
        File.Exists(filePath)
            ? filePath
            : throw new FileNotFoundException($"File not found at path '{filePath}'");

    public static FilePath AssertFilePathHasExtension(this FilePath filePath, string extension) =>
        Path.GetExtension(filePath) == extension
            ? filePath
            : throw new ArgumentException($"Path '{filePath}' does not have extension {extension}");
}
