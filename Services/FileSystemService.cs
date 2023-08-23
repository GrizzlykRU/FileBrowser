using DynamicData;
using FileBrowser.Models;
using FileBrowser.Utility;
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
                var image = IconReader.GetImage(el, true);
                elements.Add(new FileSystemElement(el, name, image));
            }
            foreach (var el in filePaths)
            {
                var name = Path.GetFileName(el);
                var image = IconReader.GetImage(el, false);
                elements.Add(new FileSystemElement(el, name, image, false));
            }

            return elements;
        }

        public IReadOnlyList<FileSystemElement> GetDrivers()
        {
            var elements = new List<FileSystemElement>();
            var drivers = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed);
            foreach(var driver in drivers)
            {
                var image = IconReader.GetImage(driver.RootDirectory.FullName, true);
                elements.Add(new FileSystemElement(driver.RootDirectory.FullName, driver.Name, image, true, true));
            }
            return elements;
        }
    }
}
