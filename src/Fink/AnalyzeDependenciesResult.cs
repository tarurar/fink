namespace Fink;

internal abstract record AnalyzeDependenciesResult : ExecutionResult;

internal abstract record AnalyzeDependenciesError : AnalyzeDependenciesResult, IExitCodeProvider
{
    public abstract int ExitCode { get; }
}

internal sealed record ConflictsDetectedError : AnalyzeDependenciesError
{
    public override int ExitCode => ExitCodes.ConflictsDetected;
}

internal sealed record BuildFailedError : AnalyzeDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;
}

internal sealed record AnalyzeDependenciesSuccess : AnalyzeDependenciesResult;