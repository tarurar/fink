using System.Resources;

namespace Fink.Abstractions;

public abstract record Result;

public interface ISuccessResult;

public interface IErrorResult
{
    string BuildOutput(ResourceManager rm) => string.Empty;
}
