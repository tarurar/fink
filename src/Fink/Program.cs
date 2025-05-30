using System.Collections.Immutable;
using System.Globalization;
using System.Resources;

using Fink.Abstractions;

using Fink.Integrations.Buildalyzer;
using Fink.Integrations.NuGet;

using NuGet.ProjectModel;

namespace Fink;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        ResourceManager rm = new("Fink.Resources", typeof(Program).Assembly);

        if (args.Length == 0)
        {
            Console.WriteLine(rm.GetString("UsageMessage", CultureInfo.InvariantCulture));
            return;
        }

        if (!args[0].EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase))
        {
            Console.WriteLine(rm.GetString("InvalidFileExtension", CultureInfo.InvariantCulture));
            return;
        }

        IEnumerable<DotNetProjectBuildResult> results = DotNetProjectBuilder.Build(
            args[0],
            new Abstractions.Environment(
                string.Empty,
                ImmutableDictionary<string, string>.Empty),
            new BuildalyzerBuildOptions(
                string.Empty,
                [args[1]], // TODO: !!!
                ImmutableList<string>.Empty,
                ImmutableList<string>.Empty));
        // ImmutableList.Create(
        //     "/p:BaseOutputPath=/Users/atarutin/RiderProjects/bookkeeper/src/BookKeeper/xxx_bin/",
        //     "/p:BaseIntermediateOutputPath=/Users/atarutin/RiderProjects/bookkeeper/src/BookKeeper/xxx_obj/")));

        DotNetProjectBuildResult result = results.First(); //TODO: !!!!

        if (result is DotNetProjectBuildError buildError)
        {
            Console.WriteLine(rm.GetString("BuildFailed", CultureInfo.InvariantCulture));
            Console.WriteLine(buildError.BuildLog);
            return;
        }

        LockFile lockFile = result.LockFilePath
            .AssertFilePathExists()
            .AssertFilePathHasExtension(".json")
            .ReadLockFile();

        List<Dependency> dependencies = [.. lockFile.GetDependenciesOrThrow(args[1])];
        List<Dependency> distinctDependencies = [.. dependencies.Distinct()];

        IEnumerable<IGrouping<DependencyName, Dependency>> multipleVersionDependencies = [.. distinctDependencies
            .GroupBy(d => d.Name)
            .Where(g => g.Count() > 1)];

        Console.WriteLine(rm.GetString("BuildSucceeded", CultureInfo.InvariantCulture));
        Console.WriteLine($"Lock file path: {lockFile.Path}");
        Console.WriteLine($"Number of dependencies: {dependencies.Count}");
        Console.WriteLine($"Number of distinct dependencies: {distinctDependencies.Count}");
        Console.WriteLine($"Number of dependencies with multiple versions: {multipleVersionDependencies.Count()}");
        foreach (IGrouping<DependencyName, Dependency> group in multipleVersionDependencies)
        {
            Console.WriteLine($"Package {group.Key} has {group.Count()} versions:");
            foreach (Dependency dependency in group)
            {
                Console.WriteLine($"  {dependency.Version} (Path: {dependency.Path})");
            }
        }
    }
}