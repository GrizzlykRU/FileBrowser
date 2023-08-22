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
    public class FileSystemElementViewModel : AbstractNotifyPropertyChanged
    {
        private IFileSystemService _fileSystemService;

        private ObservableCollection<FileSystemElementViewModel> _fileSystemElementViewModels;

        private FileSystemElement _fileSystemElement;

        private ImageSource _itemIcon;

        private bool _isOpen;

        private bool _isInitialized;

        //private readonly ReadOnlyObservableCollection<FileSystemElement> _data;


        public FileSystemElementViewModel()
        {
            _fileSystemService = new FileSystemService();
            FileSystemElementViewModels = new ObservableCollection<FileSystemElementViewModel>();
            var drivers = _fileSystemService.GetDrivers();
            FillViewModels(drivers);
        }

        public FileSystemElementViewModel(FileSystemElement fileSystemElement)
        {
            _fileSystemService = new FileSystemService();
            FileSystemElementViewModels = new ObservableCollection<FileSystemElementViewModel>();
            FileSystemElement = fileSystemElement;
            var icon = FileSystemElement.IsDirectory ? IconReader.GetFolderIcon(FileSystemElement.Path, IconReader.IconSize.Small, IconReader.FolderType.Closed) : IconReader.GetFileIcon(FileSystemElement.Path, IconReader.IconSize.Small, false);
            ItemIcon = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
            icon.Dispose();

            Open = new ViewModelCommand((sender) =>
            {
                if (FileSystemElement.IsDirectory)
                {
                    if (!_isInitialized)
                    {
                        try
                        {
                            var fileSystemElements = _fileSystemService.GetFileSystemElements(FileSystemElement.Path);
                            FillViewModels(fileSystemElements);
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
                }
                else
                {
                    Process.Start(fileSystemElement.Path);
                }
            });
            
        }

        public FileSystemElementViewModel(FileSystemElement fileSystemElement, ViewModelCommand open)
        {
            _fileSystemService = new FileSystemService();
            FileSystemElementViewModels = new ObservableCollection<FileSystemElementViewModel>();
            FileSystemElement = fileSystemElement;
            var icon = FileSystemElement.IsDirectory ? IconReader.GetFolderIcon(FileSystemElement.Path, IconReader.IconSize.Small, IconReader.FolderType.Closed) : IconReader.GetFileIcon(FileSystemElement.Path, IconReader.IconSize.Small, false);
            ItemIcon = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
            icon.Dispose();
            Open = open;
        }

        private void FillViewModels(IReadOnlyList<FileSystemElement> elements)
        {
          
            if(elements.Any())
            {
                foreach (var element in elements)
                {
                    FileSystemElementViewModels.Add(new FileSystemElementViewModel(element));
                }
            }
        }

        public FileSystemElement FileSystemElement
        {
            get => _fileSystemElement;
            set => SetAndRaise(ref _fileSystemElement, value);
        }

        public ObservableCollection<FileSystemElementViewModel> FileSystemElementViewModels
        {
            get => _fileSystemElementViewModels;
            set => SetAndRaise(ref _fileSystemElementViewModels, value);
        }

        public bool IsOpen
        {
            get => _isOpen;
            set => SetAndRaise(ref _isOpen, value);
        }

        public ImageSource ItemIcon 
        {
            get => _itemIcon;
            set => SetAndRaise(ref _itemIcon, value);
        }

        public ViewModelCommand Open { get; set; }

        //public ReadOnlyObservableCollection<FileSystemElement> Data => _data;
    }
}
