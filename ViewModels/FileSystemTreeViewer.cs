using DynamicData.Binding;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileBrowser.Models;
using FileBrowser.Services;
using System.Diagnostics;
using System;
using System.Reactive.Linq;
using System.IO;

namespace FileBrowser.ViewModels
{
    // ViewModel для отображение дерева файловой система начиная с накопителей

    public class FileSystemTreeViewer : AbstractNotifyPropertyChanged
    {
        private IFileSystemService _fileSystemService;

        private ObservableCollection<FileSystemTreeViewer> _fileSystemElementChildren;

        private MainViewModel _mainViewModel;

        private FileSystemElement _fileSystemElement;

        private bool _isOpen;

        //поле, определяющее инстанцированы потомки элемента дерева (необходимо при раскрытии/схлопывании каталога)
        private bool _isInitialized;

        private FileSystemTreeViewer(){}

        // Конструктор для корня дерева
        public FileSystemTreeViewer(MainViewModel mainViewModel)
        {
            _fileSystemService = new FileSystemService();
            _mainViewModel = mainViewModel;
            _fileSystemElementChildren = new ObservableCollection<FileSystemTreeViewer>();
            var drivers = _fileSystemService.GetDrivers();
            FillChildren(drivers);
        }

        //Конуструктор для потомков
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
                    _mainViewModel.UpdateFileSystemView(FileSystemElement.Path);
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
                else
                {
                    _mainViewModel.UpdateFileSystemView(FileSystemElement.Path);
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

        //Свойство, определяющее раскрыт ли элемент дерева
        public bool IsOpen
        {
            get => _isOpen;
            set => SetAndRaise(ref _isOpen, value);
        }

        //Команда для открытия папки или выполнения файла (выполняется по дабл клику по левой клавише мышки)
        public ViewModelCommand OpenExecute { get; set; }

        //Команда для подачи информации о файле в MainViewModel (выполняется по одиночному клику по левой клавише мышки)
        public ViewModelCommand ShowInfo { get; set; }

        //Команда для расширения/схлопывания группового элемента (папки)
        public ViewModelCommand Expand { get; set; }
    }
}
