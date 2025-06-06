using Buildalyzer;
using Buildalyzer.Environment;

using Environment = Fink.Abstractions.Environment;

namespace Fink.Integrations.Buildalyzer;

public static class ProjectBuilder
{
    public static IEnumerable<BuildalyzerBuildResult> Build(
        string projectFilePath,
        Environment environment,
        BuildalyzerBuildOptions buildOptions)
    {
        ArgumentNullException.ThrowIfNull(environment, nameof(environment));
        ArgumentNullException.ThrowIfNull(buildOptions, nameof(buildOptions));

        return Build(
                projectFilePath,
                [.. buildOptions.TargetFrameworks],
                environment.Map(buildOptions),
                out string buildLog)
            .Select(r => (BuildalyzerBuildResult)(r switch
            {
                { Succeeded: true } => new BuildalyzertBuildSuccess(
                    projectFilePath,
                    r.GetProjectAssetsFilePathOrThrow(),
                    r.TargetFramework,
                    buildLog),
                _ => new BuildalyzerBuildError(projectFilePath, buildLog)
            }));
    }

    private static IAnalyzerResults Build(
        string projectFilePath,
        string[] targetFrameworks,
        EnvironmentOptions options,
        out string buildLog)
    {
        using StringWriter buildLogWriter = new();
        IAnalyzerResults results = new AnalyzerManager(
                new AnalyzerManagerOptions { LogWriter = buildLogWriter })
            .GetProject(projectFilePath)
            .Build(targetFrameworks, options);

        buildLog = buildLogWriter.ToString();
        return results;
    }
}