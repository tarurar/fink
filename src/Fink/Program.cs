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

        var validationResult = ValidateArguments(args);
        if (validationResult != ExitCodes.Success)
        {
            PrintValidationErrorMessage(validationResult, rm);
            return validationResult;
        }

        string projectPath = args[0];
        string targetFramework = args[1];

        return AnalyzeDependencies(projectPath, targetFramework, rm);
    }

    private static int ValidateArguments(string[] args) => args switch
    {
        { Length: not 2 } => ExitCodes.UsageError,
        _ when !File.Exists(args[0]) => ExitCodes.InputFileNotFound,
        _ => ExitCodes.Success
    };

    private static void PrintValidationErrorMessage(int errorCode, ResourceManager rm)
    {
        Console.WriteLine(errorCode switch
        {
            ExitCodes.UsageError =>
                rm.GetString("UsageMessage", CultureInfo.InvariantCulture),
            ExitCodes.InputFileNotFound =>
                rm.GetString("ProjectFileNotFound", CultureInfo.InvariantCulture),
            _ => throw new InvalidOperationException($"{errorCode} is not a validation error code")
        });
    }

    private static int AnalyzeDependencies(
        string projectPath,
        string targetFramework,
        ResourceManager rm)
    {
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
            return ExitCodes.InternalError;
        }

        LockFile lockFile = result.LockFilePath
            .AssertFilePathExists()
            .AssertFilePathHasExtension(".json")
            .ReadLockFile();

        List<Dependency> dependencies = [.. lockFile.GetDependenciesOrThrow(targetFramework)];
        List<Dependency> distinctDependencies = [.. dependencies.Distinct()];

        IEnumerable<IGrouping<DependencyName, Dependency>> multipleVersionDependencies =
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
            $"Number of dependencies with multiple versions: {multipleVersionDependencies.Count()}");

        bool hasConflicts = multipleVersionDependencies.Any();

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

            return ExitCodes.ConflictsDetected;
        }

        Console.WriteLine(rm.GetString("NoConflictsFound", CultureInfo.InvariantCulture));
        return ExitCodes.Success;
    }
}