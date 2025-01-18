using Buildalyzer;

namespace Fink.Integrations.Buildalyzer;

internal static class AnalyzerResultExtensions
{
    public static string? GetProjectAssetsFilePath(this IAnalyzerResult result) =>
        result?.GetProperty("ProjectAssetsFile");

    public static string GetProjectAssetsFilePathOrThrow(this IAnalyzerResult result) =>
        result.GetProjectAssetsFilePath() ??
        throw new InvalidOperationException("Project assets file path is missing.");

    public static string? GetErrorMessage(this IAnalyzerResult result) =>
        result?.GetProperty("ErrorKeepingPropertyName");

    public static string GetErrorMessageOrThrow(this IAnalyzerResult result) =>
        result.GetErrorMessage() ??
        throw new InvalidOperationException("Build error message is missing.");
}
