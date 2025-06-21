namespace Fink.Abstractions;

public static class DependencyAnalyzer
{
    public static IReadOnlyCollection<DependencyConflict> FindVersionConflicts(
        IReadOnlyCollection<Dependency> dependencies)
    {
        var result = new List<DependencyConflict>();

        var groupedByName = dependencies.GroupBy(d => d.Name);

        foreach (var g in groupedByName)
        {
            var uniqueVersions = g
                .Select(d => d.Versioning)
                .Distinct();

            if (uniqueVersions.Count() > 1)
            {
                result.Add(
                    new DependencyConflict(
                        g.Key,
                        [.. uniqueVersions],
                        [.. g]));
            }
        }

        return result;
    }
}