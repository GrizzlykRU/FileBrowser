using System.Collections.Generic;

namespace FileBrowser.Utility
{
    //Небольшая утилита для определениая типа файла
    public static class FileExtensions
    {
        public enum FileType
        {
            Unknown = 0,
            Text = 1,
            Image = 2,
            Audio = 3,
            Video = 4
        }

        public static readonly Dictionary<FileType, List<string>> _fileExtensions = new Dictionary<FileType, List<string>>()
        {
            { FileType.Text, new List<string>(){".txt", ".css", ".csv", ".html", ".xml", ".yaml" }},
            { FileType.Image, new List<string>(){ ".bmp", ".jpeg", ".jpg", ".gif", ".svg", ".png" }},
            { FileType.Audio, new List<string>(){ ".wav", ".mid", ".oga", ".mp4a", ".mpga", ".mp3" }},
            { FileType.Video, new List<string>(){ ".3gp", ".flv", ".m4v", ".wmv", ".avi", ".mp4" }},
        };

        public static FileType GetFileTypeByExtension(string extension)
        {
            foreach(var fileType in _fileExtensions)
            {
                if (fileType.Value.Contains(extension))
                {
                    return fileType.Key;
                }
            }
            return FileType.Unknown;
        }

    }
}
