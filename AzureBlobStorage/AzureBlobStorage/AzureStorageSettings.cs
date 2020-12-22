using AzureStorageConsole.Services.Storages;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.AzureBlobStorage
{
    public class AzureStorageSettings: IStorageSettings
    {
        public string ConnectionString { get; set; }
        public string UriBase { get; set; }
        public string ContainerName { get; set; }
    }
}
