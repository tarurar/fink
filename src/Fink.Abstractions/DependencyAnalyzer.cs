using NuGet.Versioning;

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
                .Distinct()
                .ToList();

            if (uniqueVersions.Count > 1)
            {
                var (severity, firstConflictingVersion) = AnalyzeConflict(uniqueVersions);
                
                result.Add(
                    new DependencyConflict(
                        g.Key,
                        [.. uniqueVersions],
                        [.. g],
                        severity,
                        firstConflictingVersion));
            }
        }

        return result;
    }

    private static (DependencyConflictSeverity severity, DependencyVersion? firstConflictingVersion) AnalyzeConflict(
        IReadOnlyCollection<IDependencyVersioning> versions)
    {
        // For now, only implement Error level detection for major version conflicts
        var (hasMajorConflict, conflictingVersion) = FindMajorVersionConflict(versions);
        if (hasMajorConflict)
        {
            return (DependencyConflictSeverity.Error, conflictingVersion);
        }

        var (hasMinorConflict, minorConflictingVersion) = FindMinorVersionConflict(versions);
        if (hasMinorConflict)
        {
            return (DependencyConflictSeverity.Warning, minorConflictingVersion);
        }

        // TODO: Implement Info level detection for patch version conflicts
        return (DependencyConflictSeverity.Error, null);
    }

    private static (bool hasConflict, DependencyVersion? firstConflictingVersion) FindMajorVersionConflict(
        IReadOnlyCollection<IDependencyVersioning> versions)
    {
        var versionRanges = versions.Select(GetVersionRange).Where(vr => vr != null).ToList();
        
        if (versionRanges.Count < 2)
        {
            return (false, null);
        }

        // Find the range of major versions to test
        // We need to test beyond the minimum and maximum bounds to catch open-ended ranges
        var minMajorVersion = versionRanges
            .Select(vr => vr!.MinVersion?.Major ?? 0)
            .Min();
        
        var maxMajorVersion = versionRanges
            .Where(vr => vr!.MaxVersion != null)
            .Select(vr => vr!.MaxVersion!.Major)
            .DefaultIfEmpty(minMajorVersion + 10) // Default for open-ended ranges
            .Max();

        // Test a broader range to catch open-ended scenarios
        var testRangeStart = Math.Max(0, minMajorVersion - 1);
        var testRangeEnd = maxMajorVersion + 2;

        // Test each major version to find the first conflicting version
        for (var majorVersion = testRangeStart; majorVersion <= testRangeEnd; majorVersion++)
        {
            // Test both the beginning and end of the major version range
            var testVersions = new[]
            {
                new NuGetVersion(majorVersion, 0, 0),
                new NuGetVersion(majorVersion, 999999, 999999)
            };

            foreach (var testVersion in testVersions)
            {
                var satisfactionResults = versionRanges
                    .Select(vr => vr!.Satisfies(testVersion))
                    .ToList();

                // If some ranges are satisfied and some are not, it's a major version conflict
                if (satisfactionResults.Any(satisfied => satisfied) && 
                    satisfactionResults.Any(satisfied => !satisfied))
                {
                    return (true, new DependencyVersion(testVersion));
                }
            }
        }

        return (false, null);
    }

    private static (bool hasConflict, DependencyVersion? firstConflictingVersion) FindMinorVersionConflict(
        IReadOnlyCollection<IDependencyVersioning> versions)
    {
        var versionRanges = versions.Select(GetVersionRange).Where(vr => vr != null).ToList();

        if (versionRanges.Count < 2)
        {
            return (false, null);
        }

        var minMajorVersion = versionRanges
            .Select(vr => vr!.MinVersion?.Major ?? 0)
            .Min();

        var maxMajorVersion = versionRanges
            .Where(vr => vr!.MaxVersion != null)
            .Select(vr => vr!.MaxVersion!.Major)
            .DefaultIfEmpty(minMajorVersion)
            .Max();

        for (var majorVersion = minMajorVersion; majorVersion <= maxMajorVersion; majorVersion++)
        {
            var minMinorVersion = versionRanges
                .Where(vr => (vr!.MinVersion?.Major ?? 0) <= majorVersion)
                .Select(vr => vr!.MinVersion?.Major == majorVersion ? vr.MinVersion.Minor : 0)
                .DefaultIfEmpty(0)
                .Min();

            var maxMinorVersion = versionRanges
                .Where(vr => vr!.MaxVersion != null && vr.MaxVersion.Major >= majorVersion)
                .Select(vr => vr!.MaxVersion!.Major == majorVersion ? vr.MaxVersion.Minor : 999)
                .DefaultIfEmpty(minMinorVersion + 100)
                .Max();

            var testRangeStart = Math.Max(0, minMinorVersion - 1);
            var testRangeEnd = maxMinorVersion + 2;

            for (var minorVersion = testRangeStart; minorVersion <= testRangeEnd; minorVersion++)
            {
                var testVersions = new[]
                {
                    new NuGetVersion(majorVersion, minorVersion, 0),
                    new NuGetVersion(majorVersion, minorVersion, 999999)
                };

                foreach (var testVersion in testVersions)
                {
                    var satisfactionResults = versionRanges
                        .Select(vr => vr!.Satisfies(testVersion))
                        .ToList();

                    if (satisfactionResults.Any(satisfied => satisfied) &&
                        satisfactionResults.Any(satisfied => !satisfied))
                    {
                        return (true, new DependencyVersion(testVersion));
                    }
                }
            }
        }

        return (false, null);
    }

    private static VersionRange? GetVersionRange(IDependencyVersioning versioning)
    {
        return versioning switch
        {
            DependencyVersion exactVersion => 
                VersionRange.Parse($"[{exactVersion}]"),
            DependencyVersionRange versionRange => 
                ExtractVersionRange(versionRange),
            _ => null
        };
    }

    private static VersionRange? ExtractVersionRange(DependencyVersionRange versionRange)
    {
        return VersionRange.TryParse(versionRange.ToString(), out var parsedRange) 
            ? parsedRange 
            : null;
    }
}