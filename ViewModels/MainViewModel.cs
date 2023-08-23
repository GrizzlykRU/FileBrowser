using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using FileBrowser.Models;
using System.IO;
using FileBrowser.Utility;

namespace FileBrowser.ViewModels
{
    public class MainViewModel : AbstractNotifyPropertyChanged
    {
        //корень дерева файловой системы
        private FileSystemTreeViewer _treeRoot;

        //ViewModel для списка папок/файлов в выбранном каталоге
        private FileSystemListViewer _fileSystemListViewer;

        //Выбранный файл для отображения информации
        private FileSystemElement _observableFile;

        private string _search;

        //поле для отображние сообщений об ошибках или невозможности отображения файлов
        private string _filler;

        private string _filter = "";

        private bool _showFiller;

        private string _currentDirectory;

        //целочисленное значения enum FileExtensions.FileType 
        private int _filterType = 0;

        //История запросов 
        private Stack<string> _browseHistory;

        public MainViewModel()
        {
            TreeRoot = new FileSystemTreeViewer(this);
            FileSystemListViewer = new FileSystemListViewer(this);
            _browseHistory = new Stack<string>();
            Browse = new ViewModelCommand((directory) =>
                {
                    if(!string.IsNullOrEmpty(_search) && _search != _currentDirectory)
                    {
                         UpdateFileSystemView(_search);  
                    }               
                }
            );
            Back = new ViewModelCommand(
                (sender) =>
                {
                    var prevDirectory = _browseHistory.Pop();
                    UpdateFileSystemView(prevDirectory, true);
                    _currentDirectory = prevDirectory;
                    Search = _currentDirectory;
                },
                (sender) => 
                {
                    return _browseHistory.Any();
                }
            );
            FilterByFileType = new ViewModelCommand((fileType) =>
            {
                    _filterType = Convert.ToInt32(fileType);
                    if(_filterType != (int)FileExtensions.FileType.Unknown)
                    {
                        _fileSystemListViewer.FilterByExtension(_filterType);
                    }
                    else
                    {
                        _fileSystemListViewer.ResetCollectionByExtension();
                    }
                    UpdateFillerByFileVisibility();
                }
            );
        }

        //Обновляет список элементов файловой системы по выбранному пути
        //(переднные через Browse или по двйоному нажатию в списке или дереве)
        public void UpdateFileSystemView(string path, bool backPressed = false)
        {
            try
            {
                if (!backPressed && !string.IsNullOrEmpty(_currentDirectory))
                {
                    _browseHistory.Push(_currentDirectory);
                }
                _fileSystemListViewer.UpdateCollection(path);
                _currentDirectory = path;
                Search = _currentDirectory;
                if (_fileSystemListViewer.FileSystemElements.Count == 0)
                {
                    Filler = "No files";
                    ShowFiller = true;
                }
                else
                {
                    ShowFiller = false;
                    FilterByFileType.Execute(_filterType);
                    OnFilterChanged(); 
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                _currentDirectory = path;
                Search = _currentDirectory;
                Filler = "No acces";
                ShowFiller = true;
            }
            catch (DirectoryNotFoundException ex)
            {
                Filler = "Not found";
                ShowFiller = true;
            }

        }

        //Фильтрация по имени файла
        private void OnFilterChanged()
        {
            if (Filter.Length >= 3)
            {
                _fileSystemListViewer.FilterByName(Filter);
            }
            else
            {
                _fileSystemListViewer.ResetCollectionByName();
            }
            UpdateFillerByFileVisibility();
        }

        //отображение сообщения, если файлы по фильтрам не найдены и в каталоге нет папок
        private void UpdateFillerByFileVisibility()
        {
            var anyVisibleElement = FileSystemListViewer.FileSystemElements.Where(x => x.IsVisibleByName && x.IsVisibleByExtension).Any();
            if (!anyVisibleElement)
            {
                ShowFiller = true;
                Filler = "No matched files";
            }
            else
            {
                ShowFiller = false;
            }
        }

        public FileSystemTreeViewer TreeRoot
        {
            get => _treeRoot;
            set => SetAndRaise(ref _treeRoot, value);
        }

        public FileSystemListViewer FileSystemListViewer
        {
            get => _fileSystemListViewer;
            set => SetAndRaise(ref _fileSystemListViewer, value);
        }

        public FileSystemElement ObservableFile
        {
            get => _observableFile;
            set => SetAndRaise(ref _observableFile, value);
        }

        public string Search
        {
            get => _search;
            set => SetAndRaise(ref _search, value);
        }

        public string Filter
        {
            get => _filter;
            set
            {
                SetAndRaise(ref _filter, value);
                OnFilterChanged();
            }
        }

        public string Filler
        {
            get => _filler;
            set => SetAndRaise(ref _filler, value);
        }

        public bool ShowFiller
        {
            get => _showFiller;
            set => SetAndRaise(ref _showFiller, value);
        }

        //Команда для поиска 
        public ViewModelCommand Browse { get; set; }

        //Команда, восстанавливающая список элементов, опираясь на историю запросов
        public ViewModelCommand Back { get; set; }

        //Команда, выполняющая фильтрацию по типу файла
        public ViewModelCommand FilterByFileType { get; set; }

    }
}
