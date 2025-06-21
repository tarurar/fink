using NuGet.Versioning;

namespace Fink.Abstractions;

/// <summary>
/// Represents a version of a dependency in a NuGet version format
/// </summary>
public sealed record DependencyVersion :
    IComparable<DependencyVersion>,
    IEquatable<DependencyVersion>,
    IDependencyVersioning
{
    private NuGetVersion Version { get; init; }

    public DependencyVersion(string version)
    {
        Version = NuGetVersion.TryParse(version, out var nuGetVersion)
            ? nuGetVersion
            : throw new ArgumentException($"Invalid version format: {version}, expected NuGet version format.", nameof(version));
    }

    public DependencyVersion(NuGetVersion version) : base()
    {
        Version = version;
    }

    public DependencyVersion MinVersion => this;

    public override string ToString() => Version.ToString();

    /// <summary>
    /// Compares this instance with another DependencyVersion instance.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(DependencyVersion? other) => other switch
    {
        null => 1,
        _ when ReferenceEquals(this, other) => 0,
        _ => Version.CompareTo(other.Version)
    };

    public bool Equals(DependencyVersion? other) => CompareTo(other) == 0;

    public override int GetHashCode() => Version.GetHashCode();

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
}
