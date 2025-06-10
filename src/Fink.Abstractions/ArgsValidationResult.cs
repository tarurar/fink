using System.Globalization;
using System.Resources;

namespace Fink.Abstractions;

public abstract record ArgsValidationResult : Result;

public abstract record ArgsValidationError : ArgsValidationResult, IErrorResult, IExitCodeProvider, IOutputBuilder
{
    public abstract int ExitCode { get; }

    public abstract string Build(ResourceManager rm);

}

public sealed record UsageError : ArgsValidationError, IOutputBuilder
{
    public override int ExitCode => ExitCodes.UsageError;

    public override string Build(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);

        return rm.GetString("UsageMessage", CultureInfo.InvariantCulture) ??
               "Usage information is not available.";
    }
}

public sealed record ProjectFileNotFoundError(string FilePath) : ArgsValidationError, IOutputBuilder
{
    public override int ExitCode => ExitCodes.InputFileNotFound;

    public override string Build(ResourceManager rm)
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
