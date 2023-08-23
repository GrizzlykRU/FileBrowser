using DynamicData.Binding;
using System.Windows.Media;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using FileBrowser.Utility;
using System;

namespace FileBrowser.Models
{
    public class FileSystemElement : AbstractNotifyPropertyChanged
    {
        private bool _isVisibleByExtension;
        private bool _isVisibleByName;


        public FileSystemElement(string path, string name, ImageSource image, bool isFolder = true, bool isDrive = false) {
            Path = path;
            Name = name;
            Image = image;
            IsFolder = isFolder;
            IsVisibleByExtension = true;
            IsVisibleByName = true;
            if (!IsFolder)
            {
                SizeByte = new FileInfo(path).Length;
                SizeString = FileSizeConverter.GetSizeString(SizeByte);
                Extension = System.IO.Path.GetExtension(path);
                DateCreated = File.GetCreationTime(Path).ToString();
                DateLastWritten = File.GetLastWriteTime(Path).ToString();
            }
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Extension { get; set; }

        public long SizeByte { get; set; }

        public string SizeString { get; set; }

        public string DateCreated { get; set; }

        public string DateLastWritten { get; set; }

        public bool IsFolder { get; set; }

        public bool IsDrive { get; set; }

        public bool IsVisibleByExtension
        {
            get => _isVisibleByExtension;
            set => SetAndRaise(ref _isVisibleByExtension, value);
        }

        public bool IsVisibleByName
        {
            get => _isVisibleByName;
            set => SetAndRaise(ref _isVisibleByName, value);
        }

        public ImageSource Image { get; set; }

    }
}
