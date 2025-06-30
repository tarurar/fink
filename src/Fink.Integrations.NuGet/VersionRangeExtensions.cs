using NuGet.Versioning;

namespace Fink.Integrations.NuGet;

internal static class VersionRangeExtensions
{
    public static bool IsExactVersion(this VersionRange range, out NuGetVersion? version)
    {
        version = null;

        if (range.IsFloating)
        {
            return false;
        }

        if (range.HasLowerAndUpperBounds &&
            range.IsMinInclusive &&
            range.IsMaxInclusive &&
            range.MinVersion == range.MaxVersion)
        {
            version = range.MinVersion;
            return true;
        }

        return false;
    }
}
