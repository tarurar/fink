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
}