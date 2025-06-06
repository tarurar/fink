using Fink.Abstractions;

namespace Fink;

internal static class ResultLogging
{
    public static void Log(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        switch (result)
        {
            case ISuccessResult:
                Console.WriteLine($"Execution result: {result}");
                break;
            case IErrorResult errorResult:
                Console.Error.WriteLine($"Execution failed: {errorResult}");
                break;
            default:
                throw new InvalidOperationException(
                    $"Result {result.GetType().Name} must implement ISuccessResult or IErrorResult");
        }
    }
}