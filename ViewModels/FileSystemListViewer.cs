using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Linq;
using FileBrowser.Models;
using FileBrowser.Services;
using System.Diagnostics;
using FileBrowser.Utility;
using System.Reactive.Linq;
using DynamicData;

namespace FileBrowser.ViewModels
{
    //ViewModel для отображения списка файлов/каталогов в выбранном каталоге
    public class FileSystemListViewer : AbstractNotifyPropertyChanged
    {
        private IFileSystemService _fileSystemService;

        private ObservableCollection<FileSystemElement> _fileSystemElements;

        private MainViewModel _mainViewModel;


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
                    _mainViewModel.UpdateFileSystemView(element.Path);
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
            _fileSystemElements.Clear();
            var collection = _fileSystemService.GetFileSystemElements(path);
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

        //Восстанавливает видимость элементов, при сбросе фильтра по типу файла
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

        //Восстанавливает видимость элементов, при сбросе фильтра по имение
        public void ResetCollectionByName()
        {
            var files = _fileSystemElements.Where(x => !x.IsFolder && x.IsVisibleByName == false);
            foreach (var file in files)
            {
                file.IsVisibleByName = true;
            }
        }

        public ObservableCollection<FileSystemElement> FileSystemElements => _fileSystemElements;
        
        //Команда для открытия папки или выполнения файла (выполняется по дабл клику по левой клавише мышки)
        public ViewModelCommand OpenExecute { get; set; }
        
        //Команда для подачи информации о файле в MainViewModel (выполняется по одиночному клику по левой клавише мышки)
        public ViewModelCommand ShowInfo { get; set; }
    }
}
