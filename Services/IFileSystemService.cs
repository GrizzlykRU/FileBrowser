using FileBrowser.Models;
using System.Collections.Generic;

namespace FileBrowser.Services
{
    public interface IFileSystemService
    {
        IReadOnlyList<FileSystemElement> GetFileSystemElements(string path);

        IReadOnlyList<FileSystemElement> GetDrivers();
    }
}
