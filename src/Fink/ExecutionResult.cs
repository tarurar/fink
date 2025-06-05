namespace Fink;

internal abstract record ExecutionResult;

internal interface ISuccessExecutionResult
{
}

internal interface IErrorExecutionResult : IExitCodeProvider
{
}