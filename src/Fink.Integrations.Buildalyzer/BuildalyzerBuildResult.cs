using Fink.Abstractions;

namespace Fink.Integrations.Buildalyzer;

public abstract record BuildalyzerBuildResult(
    FilePath ProjectFilePath,
    string Log) : Result;

public sealed record BuildalyzerBuildError(
    FilePath ProjectFilePath,
    string Log)
    : BuildalyzerBuildResult(ProjectFilePath, Log), IErrorResult, IExitCodeProvider
{
    public int ExitCode => ExitCodes.InternalError;
}

public sealed record BuildalyzertBuildSuccess(
    FilePath ProjectFilePath,
    FilePath LockFilePath,
    string TargetFramework,
    string Log
) : BuildalyzerBuildResult(ProjectFilePath, Log), ISuccessResult;