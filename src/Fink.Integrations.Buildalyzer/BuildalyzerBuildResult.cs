using System.Resources;

using Fink.Abstractions;

namespace Fink.Integrations.Buildalyzer;

public abstract record BuildalyzerBuildResult(
    FilePath ProjectFilePath,
    string Log) : Result;

public sealed record BuildalyzerBuildError(
    FilePath ProjectFilePath,
    string Log)
    : BuildalyzerBuildResult(ProjectFilePath, Log), IErrorResult, IExitCodeProvider, IOutputBuilder
{
    public int ExitCode => ExitCodes.InternalError;

    string IOutputBuilder.Build(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);

        return Log;
    }
}

public sealed record BuildalyzerBuildSuccess(
    FilePath ProjectFilePath,
    FilePath LockFilePath,
    string TargetFramework,
    string Log
) : BuildalyzerBuildResult(ProjectFilePath, Log), ISuccessResult;