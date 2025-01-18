using Fink.Abstractions;

namespace Fink.Integrations.Buildalyzer;

public record DotNetProjectBuildError(FilePath ProjectFilePath, string TargetFramework, string BuildLog)
    : DotNetProjectBuildResult(ProjectFilePath, TargetFramework, FilePath.Empty);
