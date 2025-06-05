namespace Fink.Abstractions.ExecutionPipeline;

public abstract record ArgsValidationResult : ExecutionResult;

public abstract record ArgsValidationError : ArgsValidationResult, IErrorExecutionResult
{
    public abstract int ExitCode { get; }
}

public sealed record UsageError : ArgsValidationError
{
    public override int ExitCode => ExitCodes.UsageError;
}

public sealed record ProjectFileNotFoundError(string FilePath) : ArgsValidationError
{
    public override int ExitCode => ExitCodes.InputFileNotFound;
}

public sealed record ArgsValidationSuccess : ArgsValidationResult, ISuccessExecutionResult;
