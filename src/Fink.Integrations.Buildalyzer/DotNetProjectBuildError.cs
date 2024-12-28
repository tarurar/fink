namespace Fink.Integrations.Buildalyzer;

public record DotNetProjectBuildError(string ProjectFilePath, string TargetFramework, string ErrorMessage)
    : DotNetProjectBuildResult(ProjectFilePath, TargetFramework, string.Empty);
