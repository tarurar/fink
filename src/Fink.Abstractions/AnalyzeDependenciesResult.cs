using System.Globalization;
using System.Resources;
using System.Text;

namespace Fink.Abstractions;

public abstract record AnalyzeDependenciesResult : Result;

public abstract record AnalyzeDependenciesError : AnalyzeDependenciesResult, IErrorResult,
    IExitCodeProvider, IOutputBuilder
{
    public abstract int ExitCode { get; }

    public abstract string Build(ResourceManager rm);
}

public sealed record ConflictsDetectedError(
    IReadOnlyCollection<IGrouping<DependencyName, Dependency>> Conflicts)
    : AnalyzeDependenciesError
{
    public override int ExitCode => ExitCodes.ConflictsDetected;

    public override string Build(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);

        if (Conflicts.Count == 0)
        {
            return rm.GetString("NoConflictsFound", CultureInfo.InvariantCulture) ??
                   "No conflicts found, but the message is missing in resources.";
        }

        var conflictsFound =
            rm.GetString("ConflictsFound", CultureInfo.InvariantCulture) ??
            "Conflicts found, but the message is missing in resources.";

        var sb = new StringBuilder();
        sb.AppendLine(conflictsFound);
        foreach (IGrouping<DependencyName, Dependency> group in Conflicts)
        {
            sb.AppendLine((string)$"Package {group.Key} has {group.Count()} versions:");
            foreach (Dependency dependency in group)
            {
                sb.AppendLine((string)$"  {dependency.Version} (Path: {dependency.Path})");
            }
        }

        return sb.ToString();
    }
}

public sealed record AnalyzeDependenciesSuccess : AnalyzeDependenciesResult,
    ISuccessResult;
