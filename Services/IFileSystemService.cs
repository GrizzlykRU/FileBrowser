using FileBrowser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBrowser.Services
{
    public interface IFileSystemService
    {
        IReadOnlyList<FileSystemElement> GetFileSystemElements(string path);

        IReadOnlyList<FileSystemElement> GetDrivers();
    }
}
