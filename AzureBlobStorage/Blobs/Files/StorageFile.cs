using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.Blobs.Files
{
    public class StorageFile
    {
        public StorageFile(string fileName, long fileSize, string filePath)
        {
            FileName = fileName;
            FileSize = fileSize;
            FilePath = filePath;
        }

        public string FileName { get; private set; }
        public long FileSize { get; private set; }
        public string FilePath { get; private set; }
        public void SetFilePath(string filePath)
        {
            FilePath = filePath;
        }
    }
}
