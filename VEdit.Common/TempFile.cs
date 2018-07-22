using System;
using System.Collections.Generic;

namespace VEdit.Common
{
    [Serializable]
    public class TempFile
    {
        private static List<TempFile> _files = new List<TempFile>();

        public static void DeleteAll()
        {
            foreach (var file in _files)
            {
                file.Delete();
            }
        }

        public static string Create()
        {
            var file = new TempFile();
            _files.Add(file);
            return file.Path;
        }

        private TempFile()
        {
            Path = System.IO.Path.GetTempFileName();
        }

        public string Path { get; }

        public void Delete()
        {
            System.IO.File.Delete(Path);
        }
    }

}
