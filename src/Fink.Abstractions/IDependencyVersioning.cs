namespace Fink.Abstractions;

public interface IDependencyVersioning
{
    string ToString();
    DependencyVersion MinVersion { get; }
}