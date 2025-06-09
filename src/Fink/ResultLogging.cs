using System.Globalization;
using System.Resources;

using Fink.Abstractions;

namespace Fink;

internal static class ResultLogging
{
    private static readonly ResourceManager
        ResourceManager = new("Fink.Resources", typeof(Program).Assembly);

    public static void Log(this Result result)
    {
        switch (result)
        {
            case ISuccessResult:
                Console.WriteLine(
                    ResourceManager.GetString("ExecutionSucceeded", CultureInfo.InvariantCulture));
                break;
            case IErrorResult errorResult:
                Console.Error.Write(errorResult.BuildOutput(ResourceManager));
                break;
            default:
                throw new InvalidOperationException(
                    $"Result {result.GetType().Name} must implement ISuccessResult or IErrorResult");
        }
    }
}