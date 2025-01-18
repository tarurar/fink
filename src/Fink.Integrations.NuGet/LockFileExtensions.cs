using Fink.Abstractions;

using NuGet.ProjectModel;

namespace Fink.Integrations.NuGet;

public static class LockFileExtensions
{
    public static IEnumerable<PackageDependency> GetDependenciesOrThrow(this LockFile lockFile, string targetFramework)
    {
        ArgumentNullException.ThrowIfNull(lockFile, nameof(lockFile));

        return lockFile.Targets
            .SingleOrDefault(t => t.TargetFramework.ToString() == targetFramework)?
            .GetDependenciesOrThrow() ?? [];
    }

    internal static IEnumerable<PackageDependency> GetDependenciesOrThrow(this LockFileTarget targetFramework) =>
        targetFramework.Libraries
            .SelectMany(l =>
                l.GetDependenciesOrThrow().Append(l.MapOrThrow()));

    internal static IEnumerable<PackageDependency> GetDependenciesOrThrow(this LockFileTargetLibrary library) =>
        library.Dependencies.Select(LockFilePackageDependencyExtensions.MapOrThrow);
}
