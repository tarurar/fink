namespace Fink.Abstractions;

public abstract record CollectDependenciesResult : Result;

public abstract record CollectDependenciesError : CollectDependenciesResult, IErrorResult,
    IExitCodeProvider
{
    public abstract int ExitCode { get; }
}

public sealed record LockFileNotFoundError(string Message) : CollectDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;
}

public sealed record LockFileExtensionError(string Message) : CollectDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;
}

public sealed record LockFileParseError(string Message) : CollectDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;
}

public sealed record CollectDependenciesSuccess(IReadOnlyCollection<Dependency> Dependencies)
    : CollectDependenciesResult, ISuccessResult;
