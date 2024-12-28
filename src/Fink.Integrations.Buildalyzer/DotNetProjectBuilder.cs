using Buildalyzer;
using Buildalyzer.Environment;

using Environment = Fink.Abstractions.Environment;

namespace Fink.Integrations.Buildalyzer;

internal static class DotNetProjectBuilder
{
    public static IEnumerable<DotNetProjectBuildResult> Build(
        string projectFilePath,
        Environment environment,
        BuildalyzerBuildOptions buildOptions) =>
        Build(
                projectFilePath,
                [.. buildOptions.TargetFrameworks],
                environment.Map(buildOptions))
            .Select(buildResult => buildResult switch
            {
                { Succeeded: true } => new DotNetProjectBuildResult(
                    projectFilePath,
                    buildResult.TargetFramework,
                    buildResult.GetProjectAssetsFilePathOrThrow()),
                _ => new DotNetProjectBuildError(
                    projectFilePath,
                    buildResult.TargetFramework,
                    buildResult.GetErrorMessageOrThrow())
            });

    public static IAnalyzerResults Build(
        string projectFilePath,
        string[] targetFrameworks,
        EnvironmentOptions options) =>
        new AnalyzerManager()
            .GetProject(projectFilePath)
            .Build(targetFrameworks, options);
}