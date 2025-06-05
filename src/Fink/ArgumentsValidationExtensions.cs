using Fink.Abstractions;

namespace Fink;

internal static class ArgumentsValidationExtensions
{
    internal static ArgsValidationResult Validate(this string[] args) => args switch
    {
        { Length: not 2 } => new UsageError(),
        _ when !File.Exists(args[0]) => new ProjectFileNotFoundError(args[0]),
        _ => new ArgsValidationSuccess()
    };
}