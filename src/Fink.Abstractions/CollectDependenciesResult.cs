using System.Resources;

namespace Fink.Abstractions;

public abstract record CollectDependenciesResult : Result;

public abstract record CollectDependenciesError : CollectDependenciesResult, IErrorResult,
    IExitCodeProvider, IOutputBuilder
{
    public abstract int ExitCode { get; }
    public abstract string Build(ResourceManager rm);

}

public sealed record LockFileNotFoundError(string Message) : CollectDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;

    public override string Build(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);

        return Message;
    }
}

public sealed record LockFileExtensionError(string Message) : CollectDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;

    public override string Build(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);

        return Message;
    }
}

public sealed record LockFileParseError(string Message) : CollectDependenciesError
{
    public override int ExitCode => ExitCodes.InternalError;

    public override string Build(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);

        return Message;
    }
}

public sealed record CollectDependenciesSuccess(IReadOnlyCollection<Dependency> Dependencies)
    : CollectDependenciesResult, ISuccessResult;
