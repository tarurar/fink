namespace Fink.Abstractions;

public abstract record BuildDependenciesResult : Result;

public abstract record BuildDependenciesError : BuildDependenciesResult, IErrorResult,
    IExitCodeProvider
{
    public abstract int ExitCode { get; }
}

public sealed record BuildError(string TargetFramework, string BuildLog)
    : BuildDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;
}

public sealed record BuildDependenciesSuccess(IReadOnlyCollection<Dependency> Dependencies)
    : BuildDependenciesResult, ISuccessResult;
