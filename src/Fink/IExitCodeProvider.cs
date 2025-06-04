namespace Fink;
// todo: probably, should be moved to Fink.Console project

internal interface IExitCodeProvider
{
    int ExitCode { get; }
}