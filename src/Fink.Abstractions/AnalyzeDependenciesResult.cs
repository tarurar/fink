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
    IReadOnlyCollection<Dependency> ConflictingDependencies)
    : AnalyzeDependenciesError
{
    public override int ExitCode => ExitCodes.ConflictsDetected;

    public override string Build(ResourceManager rm)
    {
        ArgumentNullException.ThrowIfNull(rm);

        if (ConflictingDependencies.Count == 0)
        {
            return rm.GetString("NoConflictsFound", CultureInfo.InvariantCulture) ??
                   "No conflicts found, but the message is missing in resources.";
        }

        var conflictsFound =
            rm.GetString("ConflictsFound", CultureInfo.InvariantCulture) ??
            "Conflicts found, but the message is missing in resources.";

        var sb = new StringBuilder();
        sb.AppendLine(conflictsFound);
        sb.AppendLine();
        foreach (var packageNameGroup in ConflictingDependencies.GroupBy(d => d.Name))
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"Package {packageNameGroup.Key} has {packageNameGroup.DistinctBy(d => d.Version).Count()} versions:");
            foreach (var packageVersionGroup in packageNameGroup.GroupBy(d => d.Version).OrderBy(g => g.Key))
            {
                foreach (Dependency dependency in packageVersionGroup)
                {
                    sb.AppendLine(CultureInfo.InvariantCulture, $"  {dependency.Version} (Path: {dependency.Path})");
                }
            }
        }

        return sb.ToString();
    }
}

public sealed record AnalyzeDependenciesSuccess : AnalyzeDependenciesResult,
    ISuccessResult;
