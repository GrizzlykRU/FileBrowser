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
using DynamicData;

namespace FileBrowser.ViewModels
{
    public class FileSystemListViewer : AbstractNotifyPropertyChanged
    {
        private IFileSystemService _fileSystemService;

        private ObservableCollection<FileSystemElement> _fileSystemElements;

        private MainViewModel _mainViewModel;

        private ImageSource _itemIcon;

        private bool _isVisibleByName = true;

        private bool _isVisibleByExtension = true;

        private bool _isVisible = true;


        public FileSystemListViewer(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _fileSystemService = new FileSystemService();
            _fileSystemElements = new ObservableCollection<FileSystemElement>();
            OpenExecute = new ViewModelCommand((sender) =>
            {
                var element = sender as FileSystemElement;
                if (element.IsFolder)
                {
                    _mainViewModel.InitializeFileSystemView(element.Path);
                }
                else
                {
                    Process.Start(element.Path);
                }
            });

            ShowInfo = new ViewModelCommand((sender) =>
            {
                var element = sender as FileSystemElement;
                if (!element.IsFolder)
                {
                    _mainViewModel.ObservableFile = element;
                }
            });
        }

        public void UpdateCollection(string path)
        {
            var collection = _fileSystemService.GetFileSystemElements(path);
            _fileSystemElements.Clear();
            _fileSystemElements.AddRange(collection);
        }

        public void FilterByExtension(int fileType)
        {
            var files = _fileSystemElements.Where(x => !x.IsFolder);
            foreach (var file in files)
            {
                var currentFileType = FileExtensions.GetFileTypeByExtension(file.Extension);
                file.IsVisibleByExtension = (int)currentFileType == fileType;
            }
        }

        public void ResetCollectionByExtension()
        {
            var files = _fileSystemElements.Where(x => !x.IsFolder && x.IsVisibleByExtension == false);
            foreach (var file in files)
            {
                file.IsVisibleByExtension = true;
            }
        }

        public void FilterByName(string filter)
        {
            var files = _fileSystemElements.Where(x => !x.IsFolder);
            foreach (var file in files)
            {
                file.IsVisibleByName = file.Name.Contains(filter);
            }
        }

        public void ResetCollectionByName()
        {
            var files = _fileSystemElements.Where(x => !x.IsFolder && x.IsVisibleByName == false);
            foreach (var file in files)
            {
                file.IsVisibleByName = true;
            }
        }

        public ObservableCollection<FileSystemElement> FileSystemElements => _fileSystemElements;

        public bool IsVisibleByName
        {
            get => _isVisibleByName;
            set => SetAndRaise(ref _isVisibleByName, value);
        }

        public bool IsVisibleByExtension
        {
            get => _isVisibleByExtension;
            set => SetAndRaise(ref _isVisibleByExtension, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetAndRaise(ref _isVisible, value);
        }

        public ImageSource ItemIcon 
        {
            get => _itemIcon;
            set => SetAndRaise(ref _itemIcon, value);
        }

        public ViewModelCommand OpenExecute { get; set; }

        public ViewModelCommand ShowInfo { get; set; }
    }
}
