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

        return args.Validate()
            .Bind(() => DesignTimeBuild(args[0], args[1]))
            .Bind<BuildalyzerBuildSuccess>(s => Collect(s.LockFilePath, s.TargetFramework))
            .Bind<CollectDependenciesSuccess>(s => Analyze([..s.Dependencies], rm))
            .Tap(ResultLogging.Log)
            .Map(ToExitCode);
    }

    private static int ToExitCode(Result result) =>
        result switch
        {
            IExitCodeProvider provider => provider.ExitCode,
            ISuccessResult => ExitCodes.Success,
            _ => ExitCodes.Error,
        };

    private static BuildalyzerBuildResult DesignTimeBuild(
        string projectPath,
        string targetFramework) => ProjectBuilder.Build(
            projectPath,
            new Abstractions.Environment(
                string.Empty,
                ImmutableDictionary<string, string>.Empty),
            new BuildalyzerBuildOptions(
                string.Empty,
                [targetFramework],
                ImmutableList<string>.Empty,
                ImmutableList<string>.Empty))
        .First();

    private static CollectDependenciesResult Collect(FilePath lockFilePath, string targetFramework)
    {
        try
        {
            var dependencies = lockFilePath
                .AssertFilePathExists()
                .AssertFilePathHasExtension(".json")
                .ReadLockFile()
                .GetDependenciesOrThrow(targetFramework);
            return new CollectDependenciesSuccess([..dependencies]);
        }
        catch (FileNotFoundException ex)
        {
            return new LockFileNotFoundError(ex.FileName ?? lockFilePath);
        }
        catch (ArgumentException ex)
        {
            return new LockFileExtensionError(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return new LockFileParseError(ex.Message);
        }
    }

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

            return new ConflictsDetectedError(multipleVersionDependencies);
        }

        Console.WriteLine(rm.GetString("NoConflictsFound", CultureInfo.InvariantCulture));
        return new AnalyzeDependenciesSuccess();
    }
}