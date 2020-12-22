using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AzureStorageConsole.Blobs.Files
{
    internal class LocalFile
    {

        public LocalFile(string fullName)
        {
            FullName = fullName;
            Check();
        }

        private void Check()
        {
            Clear();            
            Exists = File.Exists(FullName);
            if (!Exists)
                return;
            Name = Path.GetFileNameWithoutExtension(FullName);
            Extension = Path.GetExtension(FullName);
            Folder = Path.GetDirectoryName(FullName);
        }

        private void Clear()
        {
            Name = null;
            Extension = null;
            Folder = null;
            Exists = false;
        }

        public void SetFullName(string fullName)
        {
            FullName = fullName;
            Check();
        }

        public string FullName { get; private set; }

        public string Name { get; private set; }

        public string Extension { get; private set; }

        public string Folder { get; private set; }

        public bool Exists { get; private set; }
    }
}
