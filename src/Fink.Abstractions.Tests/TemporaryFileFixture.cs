namespace Fink.Abstractions.Tests;

public sealed class TemporaryFileFixture : IDisposable
{
    public FilePath FilePath { get; }

    public TemporaryFileFixture()
    {
        FilePath = GetRandomFileName();
        File.Create(FilePath).Dispose();
    }

    public void Dispose()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }

    private static string GetRandomFileName() =>
        Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
}