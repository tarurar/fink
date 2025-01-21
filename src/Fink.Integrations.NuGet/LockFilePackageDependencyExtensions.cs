using Fink.Abstractions;

using NuGet.Versioning;

using NuGetPackageDependency = NuGet.Packaging.Core.PackageDependency;

namespace Fink.Integrations.NuGet;

internal static class LockFilePackageDependencyExtensions
{
    public static Dependency MapOrThrow(this NuGetPackageDependency dependency, Dependency parentDependency) =>
        new(
            new DependencyName(dependency.Id),
            dependency.VersionRange.GetVersionOrThrow(),
            parentDependency);

    public static DependencyVersion GetVersionOrThrow(this VersionRange versionRange) =>
        new(versionRange.OriginalString
            ?? throw new InvalidOperationException("Version range original string is null."));
}