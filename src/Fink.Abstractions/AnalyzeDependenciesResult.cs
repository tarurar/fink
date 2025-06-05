namespace Fink.Abstractions;

public abstract record AnalyzeDependenciesResult : Result;

public abstract record AnalyzeDependenciesError : AnalyzeDependenciesResult, IErrorResult,
    IExitCodeProvider
{
    public abstract int ExitCode { get; }
}

public sealed record ConflictsDetectedError : AnalyzeDependenciesError
{
    public override int ExitCode => ExitCodes.ConflictsDetected;
}

public sealed record AnalyzeDependenciesSuccess : AnalyzeDependenciesResult,
    ISuccessResult;
