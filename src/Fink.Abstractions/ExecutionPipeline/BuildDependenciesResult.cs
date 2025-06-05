namespace Fink.Abstractions.ExecutionPipeline;

public abstract record BuildDependenciesResult : ExecutionResult;

public abstract record BuildDependenciesError : BuildDependenciesResult, IErrorExecutionResult
{
    public abstract int ExitCode { get; }
}

public sealed record BuildFailedError(string TargetFramework, string BuildLog)
    : BuildDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;
}

public sealed record BuildDependenciesSuccess(IReadOnlyCollection<Dependency> Dependencies)
    : BuildDependenciesResult, ISuccessExecutionResult;
