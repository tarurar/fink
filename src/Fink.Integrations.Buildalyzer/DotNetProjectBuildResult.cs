using Fink.Abstractions;

namespace Fink.Integrations.Buildalyzer;

public record DotNetProjectBuildResult(FilePath ProjectFilePath, string TargetFramework, FilePath LockFilePath);
