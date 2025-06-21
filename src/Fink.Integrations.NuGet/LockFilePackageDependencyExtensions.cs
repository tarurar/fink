using Fink.Abstractions;

using NuGetPackageDependency = NuGet.Packaging.Core.PackageDependency;

namespace Fink.Integrations.NuGet;

internal static class LockFilePackageDependencyExtensions
{
    public static Dependency MapOrThrow(this NuGetPackageDependency dependency, Dependency parentDependency) =>
        new(
            new DependencyName(dependency.Id),
            new DependencyVersionRange(dependency.VersionRange),
            parentDependency);
}