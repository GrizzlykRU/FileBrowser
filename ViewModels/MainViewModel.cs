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
using System.Linq.Expressions;
using FileBrowser.Utility;
using System.Windows.Shapes;
using System.Reflection;
using FileBrowser.Views;

namespace FileBrowser.ViewModels
{
    public class MainViewModel : AbstractNotifyPropertyChanged
    {
        private FileSystemTreeViewer _treeRoot;
        private FileSystemListViewer _fileSystemListViewer;
        private FileSystemElement _observableFile;
        private string _search;
        private string _filler;
        private string _filter = "";
        private bool _showFiller;
        private string _currentDirectory;
        private int _filterType = 0;
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
                        try
                        {
                           InitializeFileSystemView(_search);
                            _currentDirectory = Search;
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Filler = "No acces";
                            ShowFiller = true;
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            Filler = "Not found";
                        }
                    }               
                }
            );
            Back = new ViewModelCommand(
                (sender) =>
                {
                    var prevDirectory = _browseHistory.Pop();
                    InitializeFileSystemView(prevDirectory, true);
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
                    UpdateFilterByFileVisibility();
                }
            );
        }

        public void InitializeFileSystemView(string path, bool backPressed = false)
        {
            try
            {
                _fileSystemListViewer.UpdateCollection(path);
                if (!backPressed && !string.IsNullOrEmpty(_currentDirectory))
                {
                    _browseHistory.Push(_currentDirectory);
                }
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
            catch(UnauthorizedAccessException ex) { }
            {

            }

        }

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
            UpdateFilterByFileVisibility();
        }

        private void UpdateFilterByFileVisibility()
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

        public ViewModelCommand Browse { get; set; }

        public ViewModelCommand Back { get; set; }

        public ViewModelCommand FilterByFileType { get; set; }

    }
}
