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

namespace FileBrowser.ViewModels
{
    public class MainViewModel : AbstractNotifyPropertyChanged
    {
        private string _search;
        private FileSystemElementViewModel _rootTree;
        private ObservableCollection<FileSystemElementViewModel> _fileSystemElemetsList;
        private IFileSystemService _fileSystemService;
        private string _filler;
        private bool _showFiller;
        private string _currentDirectory;
        private bool _showCurrentDirectory;
        private Stack<string> _browseHistory;

        public MainViewModel()
        {
            _fileSystemService = new FileSystemService();
            _rootTree = new FileSystemElementViewModel();
            FileSystemElemetsList = new ObservableCollection<FileSystemElementViewModel>();
            _browseHistory = new Stack<string>();
            Browse = new ViewModelCommand((directory) =>
                {
                    if(directory != null)
                    {
                        try
                        {
                           InitializeFileSystemElementsView(Search);
                           CurrentDirectory = Search;
                           ShowCurrentDirectory = true;
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Filler = "No acces";
                            ShowFiller = true;
                            ShowCurrentDirectory = false;
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            Filler = "Not found";
                            ShowCurrentDirectory = false;
                        }
                    }               
                }
            );
            Back = new ViewModelCommand((sender) =>
            {
                var prevDirectory = _browseHistory.Pop();
                var fileSystemElements = _fileSystemService.GetFileSystemElements(prevDirectory);
                FileSystemElemetsList.Clear();
                FillViewModels(fileSystemElements);
                CurrentDirectory = prevDirectory;
                ShowCurrentDirectory = true;
            },
            (sender) => 
            {
                return _browseHistory.Any();
            }
            );
        }

        private void InitializeFileSystemElementsView(string path)
        {
            var fileSystemElements = _fileSystemService.GetFileSystemElements(path);
            FileSystemElemetsList.Clear();
            FillViewModels(fileSystemElements);
            if (!string.IsNullOrEmpty(CurrentDirectory))
            {
                _browseHistory.Push(CurrentDirectory);
            }
            
        }

        private void FillViewModels(IReadOnlyCollection<FileSystemElement> elements)
        {
            if (elements.Any())
            {
                ShowFiller = false;
                foreach (var element in elements)
                {
                    var openCommand = new ViewModelCommand((directory) =>
                    {
                        var fileSystemElement = directory as FileSystemElement;
                        if (fileSystemElement.IsDirectory)
                        {
                            try
                            {
                                InitializeFileSystemElementsView(fileSystemElement.Path);
                                CurrentDirectory = fileSystemElement.Path;
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                            }
                        }
                        else
                        {
                            Process.Start(fileSystemElement.Path);
                        }
                    });
                    FileSystemElemetsList.Add(new FileSystemElementViewModel(element, openCommand));
                }
            }
            else
            {
                Filler = "No files";
                ShowFiller = true;
            }
        }

        public FileSystemElementViewModel RootTree
        {
            get => _rootTree;
            set => SetAndRaise(ref _rootTree, value);
        }

        public ObservableCollection<FileSystemElementViewModel> FileSystemElemetsList
        {
            get => _fileSystemElemetsList;
            set => SetAndRaise(ref _fileSystemElemetsList, value);
        }

        public string Search
        {
            get => _search;
            set => SetAndRaise(ref _search, value);
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

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => SetAndRaise(ref _currentDirectory, value);
        }

        public bool ShowCurrentDirectory
        {
            get => _showCurrentDirectory;
            set => SetAndRaise(ref _showCurrentDirectory, value);
        }

        public ViewModelCommand Browse { get; set; }

        public ViewModelCommand Back { get; set; }

        public ViewModelCommand FilterFileSystemElements { get; set; }


    }
}
