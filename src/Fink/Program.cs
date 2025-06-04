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
    private static int Main(string[] args) => RunApplication(args);

    private static int RunApplication(string[] args)
    {
        ResourceManager rm = new("Fink.Resources", typeof(Program).Assembly);

        ExecutionResult executionResult = args.Validate() switch
        {
            ArgsValidationError error => error,
            _ => AnalyzeDependencies(args[0], args[1], rm)
        };

        LogExecutionResult(executionResult);

        return executionResult switch
        {
            IExitCodeProvider provider => provider.ExitCode,
            _ => ExitCodes.Success
        };
    }

    private static void LogExecutionResult(ExecutionResult executionResult) =>
        Console.WriteLine($"Execution result: {executionResult}");

    private static AnalyzeDependenciesResult AnalyzeDependencies(
        string projectPath,
        string targetFramework,
        ResourceManager rm)
    {
        // todo: remove printing to console, return results with data enough to print'em later

        IEnumerable<DotNetProjectBuildResult> results = DotNetProjectBuilder.Build(
            projectPath,
            new Abstractions.Environment(
                string.Empty,
                ImmutableDictionary<string, string>.Empty),
            new BuildalyzerBuildOptions(
                string.Empty,
                [targetFramework],
                ImmutableList<string>.Empty,
                ImmutableList<string>.Empty));

        DotNetProjectBuildResult result = results.First();

        if (result is DotNetProjectBuildError buildError)
        {
            Console.WriteLine(rm.GetString("BuildFailed", CultureInfo.InvariantCulture));
            Console.WriteLine(buildError.BuildLog);
            return new BuildFailedError();
        }

        LockFile lockFile = result.LockFilePath
            .AssertFilePathExists()
            .AssertFilePathHasExtension(".json")
            .ReadLockFile();

        List<Dependency> dependencies = [.. lockFile.GetDependenciesOrThrow(targetFramework)];
        List<Dependency> distinctDependencies = [.. dependencies.Distinct()];

        List<IGrouping<DependencyName, Dependency>> multipleVersionDependencies =
        [
            .. distinctDependencies
                .GroupBy(d => d.Name)
                .Where(g => g.Count() > 1)
        ];

        Console.WriteLine(rm.GetString("BuildSucceeded", CultureInfo.InvariantCulture));
        Console.WriteLine($"Lock file path: {lockFile.Path}");
        Console.WriteLine($"Number of dependencies: {dependencies.Count}");
        Console.WriteLine($"Number of distinct dependencies: {distinctDependencies.Count}");
        Console.WriteLine(
            $"Number of dependencies with multiple versions: {multipleVersionDependencies.Count}");

        bool hasConflicts = multipleVersionDependencies.Count > 0;

        if (hasConflicts)
        {
            Console.WriteLine(rm.GetString("ConflictsFound", CultureInfo.InvariantCulture));
            foreach (IGrouping<DependencyName, Dependency> group in multipleVersionDependencies)
            {
                Console.WriteLine($"Package {group.Key} has {group.Count()} versions:");
                foreach (Dependency dependency in group)
                {
                    Console.WriteLine($"  {dependency.Version} (Path: {dependency.Path})");
                }
            }

            return new ConflictsDetectedError();
        }

        Console.WriteLine(rm.GetString("NoConflictsFound", CultureInfo.InvariantCulture));
        return new AnalyzeDependenciesSuccess();
    }
}