using System.Reflection;

using Fink.Abstractions.ExecutionPipeline;

namespace Fink.Abstractions.Tests;

public class ExecutionResultTests
{
    private static readonly Type ExecutionResultType = typeof(ExecutionResult);
    private static readonly Type SuccessInterfaceType = typeof(ISuccessExecutionResult);
    private static readonly Type ErrorInterfaceType = typeof(IErrorExecutionResult);

    [Fact]
    public void AllNonAbstractExecutionResultDescendants_ShouldImplementRequiredInterfaces()
    {
        Assembly[] finkAssemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Where(a =>
                a.GetName().Name?.StartsWith("Fink", StringComparison.OrdinalIgnoreCase) ?? true)
            .ToArray();

        List<Type> nonAbstractDescendants = [];
        foreach (var assembly in finkAssemblies)
        {
            var descedantsInAssembly = assembly.GetTypes()
                .Where(type => type.IsSubclassOf(ExecutionResultType))
                .Where(type => !type.IsAbstract)
                .ToList();

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
