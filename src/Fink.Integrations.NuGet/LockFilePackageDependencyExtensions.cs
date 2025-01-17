using PackageDependency = NuGet.Packaging.Core.PackageDependency;

namespace Fink.Integrations.NuGet;

internal static class LockFilePackageDependencyExtensions
{
    public static int GetMajorVersionOrThrow(this PackageDependency dependency) =>
        dependency.VersionRange.GetMajorVersionOrThrow();

    public static Abstractions.PackageDependency MapOrThrow(this PackageDependency dependency) =>
        new NuGetPackageMajorDependency(
            new Abstractions.PackageIdentity(dependency.Id),
            new Abstractions.PackageMajorVersion(dependency.GetMajorVersionOrThrow()));
}
