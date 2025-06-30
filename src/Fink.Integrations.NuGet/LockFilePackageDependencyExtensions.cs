using Fink.Abstractions;

using NuGet.Versioning;

using NuGetPackageDependency = NuGet.Packaging.Core.PackageDependency;

namespace Fink.Integrations.NuGet;

internal static class LockFilePackageDependencyExtensions
{
    public static Dependency MapOrThrow(this NuGetPackageDependency dependency, Dependency parentDependency)
    {
        var name = new DependencyName(dependency.Id);
        var versioning = GetVersioning(dependency.VersionRange);
        return new Dependency(name, versioning, parentDependency);
    }

    private static IDependencyVersioning GetVersioning(VersionRange versionRange) =>
        versionRange switch
        {
            { IsFloating: true } => new DependencyVersionRange(versionRange),
            var range when range.IsExactVersion(out var exactVersion) => new DependencyVersion(exactVersion!),
            _ => new DependencyVersionRange(versionRange)
        };
}