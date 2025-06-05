using Fink.Abstractions;

namespace Fink;

internal abstract record BuildDependenciesResult : ExecutionResult;

internal abstract record BuildDependenciesError : BuildDependenciesResult, IErrorExecutionResult
{
    public abstract int ExitCode { get; }
}

internal sealed record BuildFailedError(string TargetFramework, string BuildLog)
    : BuildDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;
}

internal sealed record BuildDependenciesSuccess(IReadOnlyCollection<Dependency> Dependencies)
    : BuildDependenciesResult, ISuccessExecutionResult;