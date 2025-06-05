using System.Reflection;

namespace Fink.Abstractions.Tests;

public class ResultTests
{
    private static readonly Type ResultType = typeof(Result);
    private static readonly Type SuccessInterfaceType = typeof(ISuccessResult);
    private static readonly Type ErrorInterfaceType = typeof(IErrorResult);

    [Fact]
    public void AllNonAbstractResultDescendants_ShouldImplementRequiredInterfaces()
    {
        var assemblyDirectory =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? throw new InvalidOperationException("Could not determine assembly directory.");

        List<Assembly> finkAssemblies =
            [.. Directory.GetFiles(assemblyDirectory, "Fink*.dll").Select(Assembly.LoadFrom)];

        List<Type> nonAbstractDescendants = [];
        foreach (var descedantsInAssembly in finkAssemblies.Select(assembly => assembly.GetTypes()
                     .Where(type => type.IsSubclassOf(ResultType))
                     .Where(type => !type.IsAbstract)
                     .ToList()))
        {
            nonAbstractDescendants.AddRange(descedantsInAssembly);
        }

        foreach (Type descendant in nonAbstractDescendants)
        {
            bool implementsSuccess = SuccessInterfaceType.IsAssignableFrom(descendant);
            bool implementsError = ErrorInterfaceType.IsAssignableFrom(descendant);

            // Each non-abstract descendant must implement exactly one of the two interfaces
            Assert.True(
                implementsSuccess || implementsError,
                $"Type {descendant.Name} must implement either {SuccessInterfaceType.Name} or {ErrorInterfaceType.Name}");

            // We can also verify they don't implement both (mutual exclusivity)
            Assert.False(
                implementsSuccess && implementsError,
                $"Type {descendant.Name} should not implement both {SuccessInterfaceType.Name} and {ErrorInterfaceType.Name}");
        }
    }
}
