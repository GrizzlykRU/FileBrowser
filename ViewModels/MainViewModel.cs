using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using FileBrowser.Models;
using FileBrowser.Services;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;

namespace FileBrowser.ViewModels
{
    public class MainViewModel : AbstractNotifyPropertyChanged
    {
        private const string StartDirectory = "Your computer";

        private string _search;

        private IFileSystemService _fileSystemService;

        private IReadOnlyList<FileSystemElement> _files;

        private string _currentDirectory = StartDirectory;
        
        private string _previousDirectory = StartDirectory;

        //private readonly ReadOnlyObservableCollection<FileSystemElement> _data;


        public MainViewModel()
        {
            _fileSystemService = new FileSystemService();
            //_files = _fileSystemService.GetFileSystemElements(_currentDirectory);
            _files = _fileSystemService.GetDrivers();
            Open = new ViewModelCommand((element) =>
                {
                    var el = element as FileSystemElement;
                    if (el.IsDirectory)
                    {
                        PreviousDirectory = CurrentDirectory;
                        CurrentDirectory = el.Path;
                        Data = _fileSystemService.GetFileSystemElements(CurrentDirectory);
                    }
                    else
                    {
                        Process.Start(el.Path);
                    }
                }
            );

            Back = new ViewModelCommand(
                (directory) =>
                {
                    CurrentDirectory = directory.ToString();
                    var parentDirectory = Directory.GetParent(CurrentDirectory);
                    PreviousDirectory = CurrentDirectory == StartDirectory || parentDirectory == null ? StartDirectory : parentDirectory.FullName;
                    Data = CurrentDirectory == StartDirectory ? _fileSystemService.GetDrivers() : _fileSystemService.GetFileSystemElements(CurrentDirectory);
                },
                (element) =>
                {
                    return CurrentDirectory != StartDirectory;
                }
            );

        }

        public MainViewModel(IFileSystemService fileSystemService) { 
                _fileSystemService = fileSystemService;
        }

        public string Search
        {
            get => _search;
            set => SetAndRaise(ref _search, value);
        }

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => SetAndRaise(ref _currentDirectory, value);
        }

        public string PreviousDirectory
        {
            get => _previousDirectory;
            set => SetAndRaise(ref _previousDirectory, value);
        }

        public IReadOnlyList<FileSystemElement> Data
        {
            get => _files;
            set => SetAndRaise(ref _files, value);
        }

        public ViewModelCommand Open { get; set; }

        public ViewModelCommand Back { get; set; }

        //public ReadOnlyObservableCollection<FileSystemElement> Data => _data;
    }
}
