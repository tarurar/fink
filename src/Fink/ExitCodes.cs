namespace Fink;

/// <summary>
/// Exit codes following BSD sysexits.h conventions for the Fink console application.
/// </summary>
internal static class ExitCodes
{
    /// <summary>
    /// Success - Analysis completed successfully with no dependency conflicts.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// General error - Analysis completed but detected multiple versions of the same package.
    /// This is the primary use case for Fink - dependency conflicts were found.
    /// </summary>
    public const int ConflictsDetected = 1;

    /// <summary>
    /// Usage error - Missing arguments, invalid file extension, or incorrect command line usage.
    /// </summary>
    public const int UsageError = 2;

    /// <summary>
    /// Input file not found - The specified project file doesn't exist.
    /// Based on BSD sysexits.h EX_NOINPUT (66).
    /// </summary>
    public const int InputFileNotFound = 66;

    /// <summary>
    /// Internal error - Unexpected exceptions, failing build or internal errors.
    /// Based on BSD sysexits.h EX_SOFTWARE (70).
    /// </summary>
    public const int InternalError = 70;
}
