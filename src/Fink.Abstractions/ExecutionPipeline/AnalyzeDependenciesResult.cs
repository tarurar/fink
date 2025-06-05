namespace Fink.Abstractions.ExecutionPipeline;

public abstract record AnalyzeDependenciesResult : ExecutionResult;

public abstract record AnalyzeDependenciesError : AnalyzeDependenciesResult, IErrorExecutionResult
{
    public abstract int ExitCode { get; }
}

public sealed record ConflictsDetectedError : AnalyzeDependenciesError
{
    public override int ExitCode => ExitCodes.ConflictsDetected;
}

public sealed record AnalyzeDependenciesSuccess : AnalyzeDependenciesResult,
    ISuccessExecutionResult;
