using System.Resources;

namespace Fink.Abstractions;

public interface IOutputBuilder
{
    string Build(ResourceManager rm);
}