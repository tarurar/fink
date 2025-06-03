namespace Fink;

internal abstract record ArgsValidationError;

internal sealed record None : ArgsValidationError;

internal sealed record UsageError(string Message) : ArgsValidationError;

internal sealed record ProjectFileNotFoundError(string FilePath) : ArgsValidationError;