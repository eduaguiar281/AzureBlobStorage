using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.Services.Storages
{
    public interface IStorageSettings
    {
        string ConnectionString { get; set; }
        string UriBase { get; set; }
        string ContainerName { get; set; }

    }
}
