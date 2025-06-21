using NuGet.Versioning;

namespace Fink.Abstractions;

/// <summary>
/// Represents a version range of a dependency in a NuGet version range format.
/// </summary>
public sealed record DependencyVersionRange : IEquatable<DependencyVersionRange>, IDependencyVersioning
{
    private VersionRange VersionRange { get; init; }

    public DependencyVersionRange(string versionRange)
    {
        VersionRange = VersionRange.TryParse(versionRange, out var nuGetVersionRange)
            ? nuGetVersionRange
            : throw new ArgumentException($"Invalid version range format: {versionRange}, expected NuGet version range format.", nameof(versionRange));
    }

    public DependencyVersionRange(VersionRange versionRange) : base()
    {
        VersionRange = versionRange;
    }

    public DependencyVersion MinVersion => VersionRange.MinVersion is null
        ? new DependencyVersion(new NuGetVersion(0, 0, 0))
        : new DependencyVersion(VersionRange.MinVersion);

    public override string ToString() => VersionRange.ToString();

    public bool Equals(DependencyVersionRange? other) =>
        other is not null && VersionRange.Equals(other.VersionRange);

    public override int GetHashCode() => VersionRange.GetHashCode();
}
