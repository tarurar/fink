namespace Fink.Abstractions.ExecutionPipeline;

public abstract record ExecutionResult;

// marker interface
public interface ISuccessExecutionResult;

// marker interface
public interface IErrorExecutionResult : IExitCodeProvider;
