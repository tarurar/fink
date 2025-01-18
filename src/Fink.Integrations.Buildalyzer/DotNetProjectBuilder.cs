using Buildalyzer;
using Buildalyzer.Environment;

using Environment = Fink.Abstractions.Environment;

namespace Fink.Integrations.Buildalyzer;

public static class DotNetProjectBuilder
{
    public static IEnumerable<DotNetProjectBuildResult> Build(
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
            .Select(buildResult => buildResult switch
            {
                { Succeeded: true } => new DotNetProjectBuildResult(
                    projectFilePath,
                    buildResult.TargetFramework,
                    buildResult.GetProjectAssetsFilePathOrThrow()),
                _ => new DotNetProjectBuildError(
                    projectFilePath,
                    buildResult.TargetFramework,
                    buildLog)
            });
    }

    public static IAnalyzerResults Build(
        string projectFilePath,
        string[] targetFrameworks,
        EnvironmentOptions options,
        out string buildLog)
    {
        using StringWriter buildLogWriter = new();
        IAnalyzerResults result = new AnalyzerManager(new AnalyzerManagerOptions { LogWriter = buildLogWriter })
            .GetProject(projectFilePath)
            .Build(targetFrameworks, options);

        buildLog = buildLogWriter.ToString();
        return result;
    }
}