namespace Fink.Integrations.Buildalyzer;

public record DotNetProjectBuildResult(string ProjectFilePath, string TargetFramework, string LockFilePath);
