namespace Fink.Integrations.Buildalyzer;

public record DotNetProjectBuildError(string ProjectFilePath, string TargetFramework, string BuildLog)
    : DotNetProjectBuildResult(ProjectFilePath, TargetFramework, string.Empty);
