namespace Fink.Abstractions;

public record DependencyVersion : IComparable<DependencyVersion>
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

    public override string ToString() => Version;

    public int CompareTo(DependencyVersion? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        var thisParts = ParseVersion(Version);
        var otherParts = ParseVersion(other.Version);

        // Compare major, minor, patch in order
        for (int i = 0; i < Math.Max(thisParts.Length, otherParts.Length); i++)
        {
            int thisValue = i < thisParts.Length ? thisParts[i] : 0;
            int otherValue = i < otherParts.Length ? otherParts[i] : 0;

            if (thisValue != otherValue)
            {
                return thisValue.CompareTo(otherValue);
            }
        }

        return 0;
    }

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

    private static int[] ParseVersion(string version)
    {
        if (version.StartsWith('v'))
        {
            version = version[1..].TrimStart('.');
        }

        var parts = version.Split('.');
        var result = new int[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!int.TryParse(parts[i], out result[i]))
            {
                result[i] = -1;
            }
        }

        return result;
    }

}
