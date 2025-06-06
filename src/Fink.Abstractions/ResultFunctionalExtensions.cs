namespace Fink.Abstractions;

public static class ResultFunctionalExtensions
{
    public static Result Bind(this Result result, Func<Result> next)
    {
        ArgumentNullException.ThrowIfNull(next);

        return result switch
        {
            ISuccessResult => next(),
            IErrorResult => result,
            _ => throw new InvalidOperationException(
                $"Result {result.GetType().Name} must implement ISuccessResult or IErrorResult")
        };
    }

    public static Result Bind<T>(this Result result,
        Func<T, Result> next) where T : Result
    {
        ArgumentNullException.ThrowIfNull(next);

        return result switch
        {
            T typedResult and ISuccessResult => next(typedResult),
            ISuccessResult => result,
            IErrorResult => result,
            _ => throw new InvalidOperationException(
                $"Result {result.GetType().Name} must implement ISuccessResult or IErrorResult")
        };
    }

    public static Result Tap(this Result result, Action<Result> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        action(result);
        return result;
    }
}
