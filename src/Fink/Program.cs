using System.Collections.Immutable;

using Fink.Abstractions;
using Fink.Integrations.Buildalyzer;
using Fink.Integrations.NuGet;

namespace Fink;

internal sealed class Program
{
    private static int Main(string[] args) => RunApplication(args);

    private static int RunApplication(string[] args)
    {
        return args.Validate()
            .Bind(() => DesignTimeBuild(args[0], args[1]))
            .Bind<BuildalyzerBuildSuccess>(s => Collect(s.LockFilePath, s.TargetFramework))
            .Bind<CollectDependenciesSuccess>(s => Analyze([.. s.Dependencies]))
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
            return new CollectDependenciesSuccess([.. dependencies]);
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

    private static AnalyzeDependenciesResult Analyze(List<Dependency> dependencies)
    {
        var multipleVersionDependencies = dependencies
            .GroupBy(d => new { d.Name, d.Version })
            .GroupBy(d => d.Key.Name)
            .Where(g => g.Count() > 1)
            .ToList();

        return multipleVersionDependencies.Count > 0
            ? new ConflictsDetectedError(multipleVersionDependencies.SelectMany(g => g.SelectMany(d => d)).ToList())
            : new AnalyzeDependenciesSuccess();
    }
}