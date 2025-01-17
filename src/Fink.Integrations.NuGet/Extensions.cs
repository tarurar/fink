using Fink.Abstractions;

using NuGet.ProjectModel;

namespace Fink.Integrations.NuGet;

public static class FilePathExtensions
{
    public static LockFile ReadLockFile(this FilePath lockFilePath) =>
        new LockFileFormat().Read(lockFilePath);
}