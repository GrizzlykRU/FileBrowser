using FileBrowser.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileBrowser.Models
{
    public class FileSystemElement
    {
        public FileSystemElement(string path, string name, bool isDirectory = true, bool isDrive = false) {
            Path = path;
            Name = name;
            IsDirectory = isDirectory;
            Command = new ViewModelCommand((directory) =>
            {
                PlayCommand(directory.ToString());
            });
        }

        private void PlayCommand(string str)
        {
            Console.WriteLine("directory");
        }

        public ICommand Command;

        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsDirectory { get; set; }

        public bool IsDrive { get; set; }

    }
}
