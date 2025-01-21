using Fink.Abstractions;

using NuGet.ProjectModel;

namespace Fink.Integrations.NuGet;

public static class LockFileExtensions
{
    public static IEnumerable<Dependency> GetDependenciesOrThrow(this LockFile lockFile, string targetFramework)
    {
        ArgumentNullException.ThrowIfNull(lockFile, nameof(lockFile));

        return lockFile.Targets
            .SingleOrDefault(t => t.TargetFramework.ToString() == targetFramework)?
            .GetDependenciesOrThrow() ?? [];
    }

    internal static IEnumerable<Dependency> GetDependenciesOrThrow(this LockFileTarget target)
    {
        Dependency frameworkDependency = new(new DependencyName(target.Name));

        return target.Libraries.SelectMany(library =>
        {
            Dependency libraryDependency = library.MapOrThrow(frameworkDependency);

            return library.Dependencies
                .Select(d => d.MapOrThrow(libraryDependency))
                .Append(libraryDependency);
        });
    }
}
