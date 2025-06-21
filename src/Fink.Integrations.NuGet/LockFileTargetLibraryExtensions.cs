using Fink.Abstractions;

using NuGet.ProjectModel;

namespace Fink.Integrations.NuGet;

internal static class LockFileTargetLibraryExtensions
{
    public static Dependency MapOrThrow(this LockFileTargetLibrary library, Dependency parentDependency) =>
        new(library.GetNameOrThrow(), library.GetVersioningOrThrow(), parentDependency);


    public static DependencyName GetNameOrThrow(this LockFileTargetLibrary library) =>
        new(library.Name ?? throw new InvalidOperationException("Package name is not expected to be null."));

    public static IDependencyVersioning GetVersioningOrThrow(this LockFileTargetLibrary library) =>
        new DependencyVersion(
            library.Version ?? throw new InvalidOperationException("Version is not expected to be null."));
}
