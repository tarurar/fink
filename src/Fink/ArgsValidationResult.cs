namespace Fink;

internal abstract record ArgsValidationResult : ExecutionResult;

internal abstract record ArgsValidationError : ArgsValidationResult, IExitCodeProvider
{
    public abstract int ExitCode { get; }
}

internal sealed record UsageError : ArgsValidationError
{
    public override int ExitCode => ExitCodes.UsageError;
}

internal sealed record ProjectFileNotFoundError(string FilePath) : ArgsValidationError
{
    public override int ExitCode => ExitCodes.InputFileNotFound;
}

internal sealed record ArgsValidationSuccess : ArgsValidationResult;