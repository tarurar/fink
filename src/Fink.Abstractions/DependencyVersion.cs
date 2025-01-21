namespace Fink.Abstractions;

public record DependencyVersion
{
    public string Version { get; init; }

    public DependencyVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(version));
        }

        Version = version.Trim();
    }

    public static implicit operator string(DependencyVersion version)
    {
        return version == null ? string.Empty : version.Version;
    }

    public override string ToString() => Version;
}
