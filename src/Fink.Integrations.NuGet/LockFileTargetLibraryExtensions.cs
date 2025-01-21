using NuGet.ProjectModel;
using NuGet.Versioning;

namespace Fink.Integrations.NuGet;

internal static class LockFileTargetLibraryExtensions
{
    public static Abstractions.Dependency MapOrThrow(this LockFileTargetLibrary library, Abstractions.Dependency parentDependency) =>
        new(library.GetNameOrThrow(), library.GetVersionOrThrow(), parentDependency);


    public static Abstractions.DependencyName GetNameOrThrow(this LockFileTargetLibrary library) =>
        new(library.Name ?? throw new InvalidOperationException("Package name is not expected to be null."));

    public static Abstractions.DependencyVersion GetVersionOrThrow(this LockFileTargetLibrary library) =>
        new(library.Version?.GetVersionOrThrow() ?? throw new InvalidOperationException("Version is not expected to be null."));

    public static Abstractions.DependencyVersion GetVersionOrThrow(this NuGetVersion version) =>
        new(version.OriginalVersion ?? throw new InvalidOperationException("Original version is not expected to be null."));
}
