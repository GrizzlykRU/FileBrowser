using System.Collections.Generic;

namespace FileBrowser.Utility
{
    public class FileSizeConverter
    {
        private const long Kb = 1024;
        private const long Mb = 1024 * 1024;
        private const long Gb = 1024 * 1024 * 1024;
        private static Dictionary<string, long> sizes = new Dictionary<string, long>()
        {
            { "Kb",  Kb },
            { "Mb",  Mb },
            { "Gb",  Gb },
        };

        public static string GetSizeString(long size)
        {
            var stringSize = $"{size} Bytes";
            foreach(var el in sizes)
            {
                if(size >= el.Value)
                {
                    stringSize = $"{size / el.Value} {el.Key}";
                }
            }
            return stringSize;
        }
    }
}
