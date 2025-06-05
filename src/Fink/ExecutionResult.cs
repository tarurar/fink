namespace Fink;

internal abstract record ExecutionResult;

// marker interface
internal interface ISuccessExecutionResult;

// marker interface
internal interface IErrorExecutionResult : IExitCodeProvider;