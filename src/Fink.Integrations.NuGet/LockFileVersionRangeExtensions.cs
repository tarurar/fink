using NuGet.Versioning;

namespace Fink.Integrations.NuGet;

internal static class LockFileVersionRangeExtensions
{
    // The assumption is that in a lock file, all the dependencies are always 
    // quialified with a min version.
    public static int GetMajorVersionOrThrow(this VersionRange versionRange) =>
        versionRange switch
        {
            { MinVersion: { } minVersion } => minVersion.Major,
            _ => throw new InvalidOperationException("Min version is not expected to be null.")
        };
}