using DynamicData.Binding;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileBrowser.Models;
using FileBrowser.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using FileBrowser.Utility;
using System.Windows.Interop;
using System.Drawing;
using System;
using System.Windows.Media;
using System.Reactive.Disposables;
using System.Threading;
using System.Reactive.Linq;
using ReactiveUI;
using System.IO;

namespace FileBrowser.ViewModels
{
    public class FileSystemTreeViewer : AbstractNotifyPropertyChanged
    {
        private IFileSystemService _fileSystemService;

        private ObservableCollection<FileSystemTreeViewer> _fileSystemElementChildren;

        private MainViewModel _mainViewModel;

        private FileSystemElement _fileSystemElement;

        private bool _isOpen;

        private bool _isInitialized;

        private FileSystemTreeViewer(){}

        public FileSystemTreeViewer(MainViewModel mainViewModel)
        {
            _fileSystemService = new FileSystemService();
            _mainViewModel = mainViewModel;
            _fileSystemElementChildren = new ObservableCollection<FileSystemTreeViewer>();
            var drivers = _fileSystemService.GetDrivers();
            FillChildren(drivers);
        }

        public FileSystemTreeViewer(FileSystemElement fileSystemElement, MainViewModel mainViewModel)
        {
            _fileSystemService = new FileSystemService();
            _mainViewModel = mainViewModel;
            _fileSystemElementChildren = new ObservableCollection<FileSystemTreeViewer>();
            FileSystemElement = fileSystemElement;
            if (FileSystemElement.IsFolder)
            {
                Expand = new ViewModelCommand((sender) =>
                {
                    if (!_isInitialized)
                    {
                        try
                        {
                            var fileSystemElements = _fileSystemService.GetFileSystemElements(FileSystemElement.Path);
                            FillChildren(fileSystemElements);
                        }
                        catch (UnauthorizedAccessException ex)
                        {

                        }
                        catch (DirectoryNotFoundException ex)
                        {

                        }
                        _isInitialized = true;
                    }
                    IsOpen = !IsOpen;
                });
            }

            OpenExecute = new ViewModelCommand((sender) =>
            {
                if (FileSystemElement.IsFolder)
                {
                    _mainViewModel.InitializeFileSystemView(FileSystemElement.Path);
                }
                else
                {
                    Process.Start(fileSystemElement.Path);
                }
            });

            ShowInfo = new ViewModelCommand((sender) =>
            {
                if (!FileSystemElement.IsFolder)
                {
                    _mainViewModel.ObservableFile = FileSystemElement;
                }
            });
        }

        public FileSystemTreeViewer(FileSystemElement fileSystemElement, ViewModelCommand open)
        {
            _fileSystemService = new FileSystemService();
            _fileSystemElementChildren = new ObservableCollection<FileSystemTreeViewer>();
            FileSystemElement = fileSystemElement;
            OpenExecute = open;
        }

        private void FillChildren(IReadOnlyList<FileSystemElement> elements)
        {        
            if(elements.Any())
            {
                foreach (var element in elements)
                {
                    _fileSystemElementChildren.Add(new FileSystemTreeViewer(element, _mainViewModel));
                }
            }
        }

        public FileSystemElement FileSystemElement
        {
            get => _fileSystemElement;
            set => SetAndRaise(ref _fileSystemElement, value);
        }

        public ObservableCollection<FileSystemTreeViewer> FileSystemElementChildren => _fileSystemElementChildren;

        public bool IsOpen
        {
            get => _isOpen;
            set => SetAndRaise(ref _isOpen, value);
        }

        public ViewModelCommand OpenExecute { get; set; }

        public ViewModelCommand ShowInfo { get; set; }

        public ViewModelCommand Expand { get; set; }
    }
}
