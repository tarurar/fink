using System.Collections.Immutable;
using System.Globalization;
using System.Resources;

using Fink.Abstractions;
using Fink.Integrations.Buildalyzer;
using Fink.Integrations.NuGet;

namespace Fink;

internal sealed class Program
{
    private static int Main(string[] args) => RunApplication(args);

    private static int RunApplication(string[] args)
    {
        ResourceManager rm = new("Fink.Resources", typeof(Program).Assembly);

        Result executionResult = args.Validate()
            .Bind(() => Build(args[0], args[1]))
            .Bind<BuildDependenciesSuccess>(s => Analyze([..s.Dependencies], rm));

        LogExecutionResult(executionResult);

        return executionResult switch
        {
            IExitCodeProvider provider => provider.ExitCode,
            _ => ExitCodes.Success
        };
    }

    private static void LogExecutionResult(Result executionResult) =>
        Console.WriteLine($"Execution result: {executionResult}");

    private static BuildDependenciesResult Build(
        string projectPath,
        string targetFramework) =>
        DotNetProjectBuilder.Build(
                projectPath,
                new Abstractions.Environment(
                    string.Empty,
                    ImmutableDictionary<string, string>.Empty),
                new BuildalyzerBuildOptions(
                    string.Empty,
                    [targetFramework],
                    ImmutableList<string>.Empty,
                    ImmutableList<string>.Empty)).First() switch
            {
                DotNetProjectBuildError error => new BuildError(
                    error.TargetFramework,
                    error.BuildLog),
                var success => new BuildDependenciesSuccess([
                    ..success.LockFilePath
                        .AssertFilePathExists()
                        .AssertFilePathHasExtension(".json")
                        .ReadLockFile()
                        .GetDependenciesOrThrow(targetFramework)
                ])
            };

    private static AnalyzeDependenciesResult Analyze(List<Dependency> dependencies,
        ResourceManager rm)
    {
        List<Dependency> distinctDependencies = [.. dependencies.Distinct()];
        List<IGrouping<DependencyName, Dependency>> multipleVersionDependencies =
        [
            .. distinctDependencies
                .GroupBy(d => d.Name)
                .Where(g => g.Count() > 1)
        ];

        // Console.WriteLine(rm.GetString("BuildSucceeded", CultureInfo.InvariantCulture));
        // Console.WriteLine($"Lock file path: {lockFile.Path}");
        // Console.WriteLine($"Number of dependencies: {dependencies.Count}");
        // Console.WriteLine($"Number of distinct dependencies: {distinctDependencies.Count}");
        // Console.WriteLine(
        //     $"Number of dependencies with multiple versions: {multipleVersionDependencies.Count}");

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