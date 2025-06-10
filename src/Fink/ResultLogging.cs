using System.Globalization;
using System.Resources;

using Fink.Abstractions;

namespace Fink;

internal static class ResultLogging
{
    private static readonly ResourceManager ResourceManager =
        new("Fink.Resources", typeof(Program).Assembly);

    public static void Log(this Result result)
    {
        switch (result)
        {
            case IOutputBuilder outputBuilder and ISuccessResult:
                Console.Write(outputBuilder.Build(ResourceManager) ?? GetSuccessMessage());
                break;
                
            case IOutputBuilder outputBuilder and IErrorResult errorResult:
                Console.Error.Write(outputBuilder.Build(ResourceManager) ?? GetStubErrorMessage(errorResult));
                break;
                
            case ISuccessResult:
                Console.Write(GetSuccessMessage());
                break;
                
            case IErrorResult errorResult:
                Console.Error.Write(GetStubErrorMessage(errorResult));
                break;
                
            default:
                throw new InvalidOperationException(
                    $"Result {result.GetType().Name} must implement ISuccessResult or IErrorResult");
        }
    }

    private static string GetSuccessMessage() =>
        ResourceManager.GetString("ExecutionSucceeded", CultureInfo.InvariantCulture) ?? 
        "Execution succeeded, but the message is missing in resources.";

    private static string GetStubErrorMessage(IErrorResult errorResult) =>
        $"Execution failed, but no additional error message is provided: {errorResult}.";
}