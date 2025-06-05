namespace Fink.Abstractions.ExecutionPipeline;

public static class ExecutionResultFunctionalExtensions
{
    public static ExecutionResult Bind(this ExecutionResult result, Func<ExecutionResult> next)
    {
        ArgumentNullException.ThrowIfNull(next);

        return result switch
        {
            ISuccessExecutionResult => next(),
            IErrorExecutionResult => result,
            _ => throw new InvalidOperationException(
                $"Result {result.GetType().Name} must implement ISuccessResult or IErrorResult")
        };
    }

    public static ExecutionResult Bind<T>(this ExecutionResult result,
        Func<T, ExecutionResult> next) where T : ExecutionResult
    {
        ArgumentNullException.ThrowIfNull(next);

        return result switch
        {
            T typedResult and ISuccessExecutionResult => next(typedResult),
            ISuccessExecutionResult => result,
            IErrorExecutionResult => result,
            _ => throw new InvalidOperationException(
                $"Result {result.GetType().Name} must implement ISuccessResult or IErrorResult")
        };
    }
}
