namespace Fink.Abstractions;

public abstract record ArgsValidationResult : Result;

public abstract record ArgsValidationError : ArgsValidationResult, IErrorResult, IExitCodeProvider
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

public sealed record ArgsValidationSuccess : ArgsValidationResult, ISuccessResult;
