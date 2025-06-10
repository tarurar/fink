namespace Fink.Abstractions;

public static class ResultFunctionalExtensions
{
    /// <summary>
    /// Executes the provided function if the result represents a success, otherwise returns the original result.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="next">The function to execute if the result is successful.</param>
    /// <returns>The result of the next function if successful, otherwise the original result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when next is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the result doesn't implement ISuccessResult or IErrorResult.</exception>
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

    /// <summary>
    /// Executes the provided function with the typed result if the result represents a success and matches the specified type,
    /// otherwise returns the original result.
    /// </summary>
    /// <typeparam name="T">The specific result type to match and bind.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="next">The function to execute with the typed result if successful and of type T.</param>
    /// <returns>The result of the next function if successful and of type T, otherwise the original result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when next is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the result doesn't implement ISuccessResult or IErrorResult.</exception>
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

    /// <summary>
    /// Transforms the result into a value of type T using the provided mapper function.
    /// </summary>
    /// <typeparam name="T">The type to map the result to.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <param name="mapper">The function to transform the result into type T.</param>
    /// <returns>The value returned by the mapper function.</returns>
    /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
    public static T Map<T>(this Result result, Func<Result, T> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return mapper(result);
    }

    /// <summary>
    /// Executes the provided action on the result and returns the result unchanged.
    /// This is useful for side effects like logging or debugging without modifying the result.
    /// </summary>
    /// <param name="result">The result to tap into.</param>
    /// <param name="action">The action to execute with the result.</param>
    /// <returns>The original result unchanged.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    public static Result Tap(this Result result, Action<Result> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        action(result);
        return result;
    }
}
