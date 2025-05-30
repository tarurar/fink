using System.Resources;

using Microsoft.Build.Framework;

using Task = Microsoft.Build.Utilities.Task;

namespace Fink.Build;

public class GenerateResourceTask : Task
{
    [Required]
    public string SourceFile { get; set; } = string.Empty;

    [Required]
    public string OutputFile { get; set; } = string.Empty;

    public override bool Execute()
    {
        try
        {
            return ValidateSourceFile() && GenerateResourceFile();
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex);
            return false;
        }
    }

    private bool ValidateSourceFile()
    {
        if (File.Exists(SourceFile))
        {
            return true;
        }

        Log.LogError($"Source file not found: {SourceFile}");
        return false;
    }

    private bool GenerateResourceFile()
    {
        using var writer = new ResourceWriter(OutputFile);

        ProcessSourceLines(writer);
        writer.Generate();

        Log.LogMessage(MessageImportance.Normal, $"Generated resource file: {OutputFile}");
        return true;
    }

    private void ProcessSourceLines(ResourceWriter writer)
    {
        var lines = ReadSourceFileLines();
        var lineData = CreateLineData(lines);

        foreach (var (line, lineNumber) in lineData)
        {
            ProcessLine(writer, line, lineNumber);
        }
    }

    private string[] ReadSourceFileLines() => File.ReadAllLines(SourceFile);

    private static IEnumerable<(string Line, int LineNumber)> CreateLineData(string[] lines) =>
        lines.Select((line, index) => (line, index + 1));

    private void ProcessLine(ResourceWriter writer, string line, int lineNumber)
    {
        var trimmed = line.Trim();

        if (ShouldSkipLine(trimmed))
        {
            return;
        }

        var keyValue = ParseKeyValue(trimmed, line, lineNumber);
        if (keyValue.HasValue)
        {
            writer.AddResource(keyValue.Value.Key, keyValue.Value.Value);
        }
    }

    private static bool ShouldSkipLine(string trimmed) =>
        string.IsNullOrEmpty(trimmed) || IsCommentLine(trimmed);

    private static bool IsCommentLine(string line) =>
        line.StartsWith("#", StringComparison.OrdinalIgnoreCase) ||
        line.StartsWith(";", StringComparison.OrdinalIgnoreCase);

    private (string Key, string Value)? ParseKeyValue(string trimmed, string originalLine, int lineNumber)
    {
        var parts = trimmed.Split(['='], 2);

        if (!IsValidKeyValueFormat(parts))
        {
            Log.LogWarning($"Invalid format at line {lineNumber} in {SourceFile}: {originalLine}");
            return null;
        }

        var (key, value) = ExtractKeyValue(parts);

        if (string.IsNullOrEmpty(key))
        {
            Log.LogWarning($"Empty key at line {lineNumber} in {SourceFile}: {originalLine}");
            return null;
        }

        return (key, value);
    }

    private static bool IsValidKeyValueFormat(string[] parts) => parts.Length == 2;

    private static (string Key, string Value) ExtractKeyValue(string[] parts) =>
        (parts[0].Trim(), parts[1].Trim());
}