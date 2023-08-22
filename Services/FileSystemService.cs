using DynamicData;
using FileBrowser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileBrowser.Services
{
    public class FileSystemService : IFileSystemService
    {
        public IReadOnlyList<FileSystemElement> GetFileSystemElements(string path)
        {
            var elements = new List<FileSystemElement>();
            var directoryPaths = Directory.GetDirectories(path);
            var filePaths = Directory.GetFiles(path);
            Array.Sort(directoryPaths);
            Array.Sort(filePaths);
            foreach (var el in directoryPaths)
            {
                var name = Path.GetFileName(el);
                elements.Add(new FileSystemElement(el, name));
            }
            foreach (var el in filePaths)
            {
                var name = Path.GetFileName(el);
                elements.Add(new FileSystemElement(el, name, false));
            }

            return elements;
        }

        public IReadOnlyList<FileSystemElement> GetDrivers()
        {
            var elements = new List<FileSystemElement>();
            var drivers = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed);
            foreach(var driver in drivers)
            {
                elements.Add(new FileSystemElement(driver.RootDirectory.FullName, driver.Name, true, true));
            }
            return elements;
        }
    }
}
