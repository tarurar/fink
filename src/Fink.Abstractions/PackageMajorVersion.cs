namespace Fink.Abstractions;

public sealed record PackageMajorVersion(int MajorVersion)
{
    public static PackageMajorVersion Create(int majorVersion) =>
        majorVersion switch
        {
            < 0 => throw new ArgumentOutOfRangeException(nameof(majorVersion)),
            _ => new PackageMajorVersion(majorVersion)
        };
}