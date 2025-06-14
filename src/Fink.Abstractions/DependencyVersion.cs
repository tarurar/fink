using Semver;

namespace Fink.Abstractions;

/// <summary>
/// Represents a version of a dependency in a semantic version format.
/// </summary>
public sealed record DependencyVersion : IComparable<DependencyVersion>, IEquatable<DependencyVersion>
{
    /// <summary>
    /// Original version string
    /// </summary>
    public string Version { get; init; }

    /// <summary>
    /// Parsed semantic version
    /// </summary>
    private SemVersion SemVersion { get; init; }

    public DependencyVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(version));
        }

        if (!SemVersion.TryParse(version, SemVersionStyles.Any, out var semVersion))
        {
            throw new ArgumentException($"Invalid version format: {version}, expected semantic version format.", nameof(version));
        }

        SemVersion = semVersion;
        Version = version;
    }

    public override string ToString() => Version;

    public int CompareTo(DependencyVersion? other) => other switch
    {
        null => 1,
        _ when ReferenceEquals(this, other) => 0,
        _ => SemVersion.ComparePrecedenceTo(other.SemVersion)
    };

    public bool Equals(DependencyVersion? other) => CompareTo(other) == 0;

    public override int GetHashCode() => SemVersion.GetHashCode();

    public static bool operator <(DependencyVersion left, DependencyVersion right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(DependencyVersion left, DependencyVersion right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(DependencyVersion left, DependencyVersion right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(DependencyVersion left, DependencyVersion right)
    {
        ArgumentNullException.ThrowIfNull(left);

        return left.CompareTo(right) >= 0;
    }

    public static implicit operator string(DependencyVersion version)
    {
        return version == null ? string.Empty : version.Version;
    }
}
