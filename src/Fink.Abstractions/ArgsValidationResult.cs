using System.Globalization;
using System.Resources;

namespace Fink.Abstractions;

public abstract record ArgsValidationResult : Result;

public abstract record ArgsValidationError : ArgsValidationResult, IErrorResult, IExitCodeProvider
{
    public abstract int ExitCode { get; }
    public abstract string BuildOutput(ResourceManager rm);
}

public sealed record UsageError : ArgsValidationError
{
    public override int ExitCode => ExitCodes.UsageError;

    public override string BuildOutput(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);

        return rm.GetString("UsageMessage", CultureInfo.InvariantCulture) ??
               "Usage information is not available.";
    }
}

public sealed record ProjectFileNotFoundError(string FilePath) : ArgsValidationError
{
    public override int ExitCode => ExitCodes.InputFileNotFound;

    public override string BuildOutput(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);
        var projectFileNotFoundFmt =
            rm.GetString("ProjectFileNotFoundFmt", CultureInfo.InvariantCulture);
        return projectFileNotFoundFmt is null
            ? "Project file not found, but the format string is missing in resources."
            : string.Format(CultureInfo.InvariantCulture, projectFileNotFoundFmt, FilePath);
    }
}

public sealed record ArgsValidationSuccess : ArgsValidationResult, ISuccessResult;
