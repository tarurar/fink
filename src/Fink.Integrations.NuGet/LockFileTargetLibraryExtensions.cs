using NuGet.ProjectModel;

namespace Fink.Integrations.NuGet;

internal static class LockFileTargetLibraryExtensions
{
    public static Abstractions.PackageDependency MapOrThrow(this LockFileTargetLibrary library) =>
        new NuGetPackageMajorDependency(
            new Abstractions.PackageIdentity(library.Name ?? throw new InvalidOperationException("Package name is not expected to be null.")),
            new Abstractions.PackageMajorVersion(library.Version?.Major ?? throw new InvalidOperationException($"Package [{library.Name}] version is not expected to be null")));
}
