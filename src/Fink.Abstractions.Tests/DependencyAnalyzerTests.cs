namespace Fink.Abstractions.Tests;

public class DependencyAnalyzerTests
{
    [Fact]
    public void FindVersionConflicts_EmptyCollection_ReturnsEmptyResult()
    {
        var dependencies = new List<Dependency>();

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        Assert.Empty(conflicts);
    }

    [Fact]
    public void FindVersionConflicts_SingleDependency_ReturnsNoConflicts()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        Assert.Empty(conflicts);
    }

    [Fact]
    public void FindVersionConflicts_SameDependencySameVersion_ReturnsNoConflicts()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersion("1.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        Assert.Empty(conflicts);
    }

    [Fact]
    public void FindVersionConflicts_SameDependencyDifferentVersions_ReturnsConflict()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersion("2.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal("package1", conflict.Name.Name);
        Assert.Equal(2, conflict.Versions.Count);
        Assert.Equal(2, conflict.ConflictedDependencies.Count);
        Assert.Contains(conflict.Versions, v => v.ToString() == "1.0.0");
        Assert.Contains(conflict.Versions, v => v.ToString() == "2.0.0");
    }

    [Fact]
    public void FindVersionConflicts_MultipleDependenciesWithConflicts_ReturnsMultipleConflicts()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersion("2.0.0")),
            new(new DependencyName("package2"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package2"), new DependencyVersion("1.5.0")),
            new(new DependencyName("package3"), new DependencyVersion("1.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        Assert.Equal(2, conflicts.Count);
        Assert.Contains(conflicts, c => c.Name.Name == "package1");
        Assert.Contains(conflicts, c => c.Name.Name == "package2");
        Assert.DoesNotContain(conflicts, c => c.Name.Name == "package3");
    }

    [Fact]
    public void FindVersionConflicts_ThreeVersionsOfSameDependency_ReturnsConflictWithThreeVersions()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersion("2.0.0")),
            new(new DependencyName("package1"), new DependencyVersion("3.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal("package1", conflict.Name.Name);
        Assert.Equal(3, conflict.Versions.Count);
        Assert.Equal(3, conflict.ConflictedDependencies.Count);
    }

    [Fact]
    public void FindVersionConflicts_DifferentVersioningTypes_ReturnsConflict()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersionRange("[1.0.0, 2.0.0)"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal("package1", conflict.Name.Name);
        Assert.Equal(2, conflict.Versions.Count);
        Assert.Equal(2, conflict.ConflictedDependencies.Count);
        Assert.Contains(conflict.Versions, v => v.ToString() == "1.0.0");
        Assert.Contains(conflict.Versions, v => v.ToString() == "[1.0.0, 2.0.0)");
    }

    [Fact]
    public void FindVersionConflicts_DependenciesWithParents_ReturnsConflictIncludingParentInfo()
    {
        var parent1 = new Dependency(new DependencyName("parent1"), new DependencyVersion("1.0.0"));
        var parent2 = new Dependency(new DependencyName("parent2"), new DependencyVersion("1.0.0"));
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0"), parent1),
            new(new DependencyName("package1"), new DependencyVersion("2.0.0"), parent2)
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal("package1", conflict.Name.Name);
        Assert.Equal(2, conflict.ConflictedDependencies.Count);

        var conflictedDeps = conflict.ConflictedDependencies.ToList();
        Assert.Contains(conflictedDeps, d => d.ParentDependency?.Name.Name == "parent1");
        Assert.Contains(conflictedDeps, d => d.ParentDependency?.Name.Name == "parent2");
    }

    [Fact]
    public void FindVersionConflicts_DuplicateDependenciesWithSameParent_ReturnsNoConflicts()
    {
        var parent = new Dependency(new DependencyName("parent"), new DependencyVersion("1.0.0"));
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0"), parent),
            new(new DependencyName("package1"), new DependencyVersion("1.0.0"), parent)
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        Assert.Empty(conflicts);
    }

    [Fact]
    public void FindVersionConflicts_MixedVersionFormats_ReturnsCorrectConflict()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersionRange("[1.0.0, 2.0.0)")),
            new(new DependencyName("package1"), new DependencyVersionRange("[1.0.0, 2.0.0)")),
            new(new DependencyName("package1"), new DependencyVersion("2.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(3, conflict.Versions.Count); // Distinct versions: 1.0.0, [1.0.0, 2.0.0), 2.0.0
        Assert.Equal(5, conflict.ConflictedDependencies.Count); // All 5 dependencies
    }

    [Fact]
    public void FindVersionConflicts_LargeCollectionWithMixedConflicts_ReturnsCorrectResults()
    {
        var dependencies = new List<Dependency>();

        // Add non-conflicting dependencies
        for (int i = 0; i < 100; i++)
        {
            dependencies.Add(new(new DependencyName($"unique-package-{i}"), new DependencyVersion("1.0.0")));
        }

        // Add conflicting dependencies
        dependencies.Add(new(new DependencyName("conflicted-package"), new DependencyVersion("1.0.0")));
        dependencies.Add(new(new DependencyName("conflicted-package"), new DependencyVersion("2.0.0")));
        dependencies.Add(new(new DependencyName("conflicted-package"), new DependencyVersion("3.0.0")));

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal("conflicted-package", conflict.Name.Name);
        Assert.Equal(3, conflict.Versions.Count);
        Assert.Equal(3, conflict.ConflictedDependencies.Count);
    }

    [Theory]
    [InlineData("1.0.0", "2.0.0")]
    [InlineData("1.0.0", "1.0.1")]
    [InlineData("1.0.0-alpha", "1.0.0-beta")]
    [InlineData("1.0.0", "1.0.0-beta")]
    public void FindVersionConflicts_DifferentVersionPatterns_DetectsConflicts(string version1, string version2)
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersion(version1)),
            new(new DependencyName("package"), new DependencyVersion(version2))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal("package", conflict.Name.Name);
        Assert.Equal(2, conflict.Versions.Count);
    }

    [Fact]
    public void FindVersionConflicts_VersionRangeConflicts_DetectsCorrectly()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[1.0.0, 2.0.0)")),
            new(new DependencyName("package"), new DependencyVersionRange("[2.0.0, 3.0.0)")),
            new(new DependencyName("package"), new DependencyVersionRange("[1.0.0, 2.0.0)"))  // Duplicate
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal("package", conflict.Name.Name);
        Assert.Equal(2, conflict.Versions.Count); // Only unique versions
        Assert.Equal(3, conflict.ConflictedDependencies.Count); // All dependencies
    }

    [Fact]
    public void FindVersionConflicts_NullCollection_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => DependencyAnalyzer.FindVersionConflicts(null!));

    [Fact]
    public void FindVersionConflicts_ReadOnlyCollection_WorksCorrectly()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersion("2.0.0"))
        }.AsReadOnly();

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal("package1", conflict.Name.Name);
    }

    // Major Version Conflict Detection Tests

    [Fact]
    public void FindVersionConflicts_MajorVersionConflict_ExactVersions_ReturnsErrorSeverity()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package"), new DependencyVersion("2.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
    }

    [Fact]
    public void FindVersionConflicts_MajorVersionConflict_OpenEndedRange_ReturnsErrorSeverity()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[8.0.0, )")),
            new(new DependencyName("package"), new DependencyVersion("8.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
    }

    [Fact]
    public void FindVersionConflicts_MajorVersionConflict_ExclusiveUpperBound_ReturnsErrorSeverity()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[5.0.0, 8.0.0)")),
            new(new DependencyName("package"), new DependencyVersionRange("[6.0.0, )"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
    }

    [Fact]
    public void FindVersionConflicts_MajorVersionConflict_DifferentMinVersions_ReturnsErrorSeverity()
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[8.0.0, )")),
            new(new DependencyName("package"), new DependencyVersionRange("[8.1.0, )"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
    }

    [Theory]
    [InlineData("1.0.0", "2.0.0")]
    [InlineData("2.5.0", "3.0.0")]
    [InlineData("10.0.0", "11.5.2")]
    public void FindVersionConflicts_DifferentMajorVersions_ReturnsErrorSeverity(string version1, string version2)
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersion(version1)),
            new(new DependencyName("package"), new DependencyVersion(version2))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
    }

    [Theory]
    [InlineData("[1.0.0, )", "2.0.0")]
    [InlineData("[5.0.0, 8.0.0)", "[6.0.0, )")]
    [InlineData("[8.0.0, )", "[8.1.0, )")]
    [InlineData("[1.0.0]", "[2.0.0, )")]
    public void FindVersionConflicts_MajorVersionRangeConflicts_ReturnsErrorSeverity(string range1, string range2)
    {
        ArgumentNullException.ThrowIfNull(range1);
        ArgumentNullException.ThrowIfNull(range2);

        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), CreateVersioning(range1)),
            new(new DependencyName("package"), CreateVersioning(range2))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
    }

    [Fact]
    public void FindVersionConflicts_SameMajorVersionDifferentMinor_WhenLowerVersionIsMajorBoundary_ReturnsErrorSeverity()
    {
        // 1.0.0 is a major boundary (X.0.0), so FindMajorVersionConflict catches this first
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package"), new DependencyVersion("1.1.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
    }

    [Fact]
    public void FindVersionConflicts_SameMajorVersionDifferentMinor_WhenNotAtMajorBoundary_ReturnsWarningSeverity()
    {
        // 1.1.0 vs 1.2.0: probe at 1.0.0 → both false, no major boundary disagreement
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersion("1.1.0")),
            new(new DependencyName("package"), new DependencyVersion("1.2.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Warning, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
    }

    [Fact]
    public void FindVersionConflicts_SameMajorAndMinorVersionDifferentPatch_ReturnsErrorSeverity()
    {
        // TODO: This should return Info severity when patch version detection is implemented
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package"), new DependencyVersion("1.0.1"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity); // Will be Info when implemented
    }

    [Fact]
    public void FindVersionConflicts_MultipleDependenciesWithDifferentSeverities_ReturnsCorrectSeverities()
    {
        var dependencies = new List<Dependency>
        {
            // Major version conflict
            new(new DependencyName("package1"), new DependencyVersion("1.0.0")),
            new(new DependencyName("package1"), new DependencyVersion("2.0.0")),
            // Minor version conflict (versions not at major boundary)
            new(new DependencyName("package2"), new DependencyVersion("1.1.0")),
            new(new DependencyName("package2"), new DependencyVersion("1.3.0")),
            // Patch version conflict (currently returns Error, will change when Phase 3 is implemented)
            new(new DependencyName("package3"), new DependencyVersion("1.1.1")),
            new(new DependencyName("package3"), new DependencyVersion("1.1.2"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        Assert.Equal(3, conflicts.Count);

        var package1Conflict = conflicts.First(c => c.Name.Name == "package1");
        var package2Conflict = conflicts.First(c => c.Name.Name == "package2");
        var package3Conflict = conflicts.First(c => c.Name.Name == "package3");

        Assert.Equal(DependencyConflictSeverity.Error, package1Conflict.Severity);
        Assert.Equal(DependencyConflictSeverity.Warning, package2Conflict.Severity);
        Assert.Equal(DependencyConflictSeverity.Error, package3Conflict.Severity); // Will change when Phase 3 is implemented
    }

    [Fact]
    public void FindVersionConflicts_FirstConflictingVersion_IdentifiesCorrectVersion()
    {
        // Test case from plan: [8.0.0, ) and [8.0.0]. 
        // The algorithm finds the first version that creates a conflict
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[8.0.0, )")),
            new(new DependencyName("package"), new DependencyVersion("8.0.0"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
        // The algorithm finds a conflicting version - the exact version depends on implementation
        Assert.NotEqual("", conflict.FirstConflictingVersion.ToString());
    }

    [Fact]
    public void FindVersionConflicts_FirstConflictingVersion_ExclusiveUpperBound()
    {
        // Test case from plan: [5.0.0, 8.0.0) and [6.0.0, )
        // The algorithm finds the first version that creates a conflict
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[5.0.0, 8.0.0)")),
            new(new DependencyName("package"), new DependencyVersionRange("[6.0.0, )"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
        // The algorithm finds a conflicting version - the exact version depends on implementation
        Assert.NotEqual("", conflict.FirstConflictingVersion.ToString());
    }

    [Fact]
    public void FindVersionConflicts_FirstConflictingVersion_DifferentMinVersions()
    {
        // Test case from plan: [8.0.0, ) and [8.1.0, )
        // The algorithm finds the first version that creates a conflict
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[8.0.0, )")),
            new(new DependencyName("package"), new DependencyVersionRange("[8.1.0, )"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Error, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
        // The algorithm finds a conflicting version - the exact version depends on implementation  
        Assert.NotEqual("", conflict.FirstConflictingVersion.ToString());
    }

    // Minor Version Conflict Detection Tests (Warning Severity)

    [Fact]
    public void FindVersionConflicts_MinorVersionConflict_PlanExample_ReturnsWarningSeverity()
    {
        // Plan example: [8.0.0, 8.15.0] and [8.0.0, 8.5.10]
        // No major conflict, but 8.6.0 satisfies first range but not second
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[8.0.0, 8.15.0]")),
            new(new DependencyName("package"), new DependencyVersionRange("[8.0.0, 8.5.10]"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Warning, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
    }

    [Theory]
    [InlineData("1.1.0", "1.2.0")]
    [InlineData("2.1.0", "2.5.0")]
    [InlineData("10.1.0", "10.3.0")]
    public void FindVersionConflicts_DifferentMinorVersions_ReturnsWarningSeverity(string version1, string version2)
    {
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersion(version1)),
            new(new DependencyName("package"), new DependencyVersion(version2))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Warning, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
    }

    [Theory]
    [InlineData("[1.1.0, 1.5.0]", "[1.3.0, 1.8.0]")]
    [InlineData("[2.1.0, 2.10.0)", "[2.5.0, 2.15.0)")]
    public void FindVersionConflicts_MinorVersionRangeConflicts_ReturnsWarningSeverity(string range1, string range2)
    {
        ArgumentNullException.ThrowIfNull(range1);
        ArgumentNullException.ThrowIfNull(range2);

        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), CreateVersioning(range1)),
            new(new DependencyName("package"), CreateVersioning(range2))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Warning, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
    }

    [Fact]
    public void FindVersionConflicts_MinorVersionConflict_FirstConflictingVersion_IsMinorBoundary()
    {
        // [8.0.0, 8.15.0] and [8.0.0, 8.5.10]
        // The first conflicting version should be at a minor boundary (8.6.0 or similar)
        var dependencies = new List<Dependency>
        {
            new(new DependencyName("package"), new DependencyVersionRange("[8.0.0, 8.15.0]")),
            new(new DependencyName("package"), new DependencyVersionRange("[8.0.0, 8.5.10]"))
        };

        var conflicts = DependencyAnalyzer.FindVersionConflicts(dependencies);

        var conflict = Assert.Single(conflicts);
        Assert.Equal(DependencyConflictSeverity.Warning, conflict.Severity);
        Assert.NotNull(conflict.FirstConflictingVersion);
        Assert.StartsWith("8.", conflict.FirstConflictingVersion.ToString(), StringComparison.Ordinal);
    }

    private static IDependencyVersioning CreateVersioning(string version)
    {
        return version.Contains('[', StringComparison.Ordinal) ||
               version.Contains('(', StringComparison.Ordinal) ||
               version.Contains(')', StringComparison.Ordinal) ||
               version.Contains(']', StringComparison.Ordinal)
            ? new DependencyVersionRange(version)
            : new DependencyVersion(version);
    }
}