using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.Blobs.Services.Table
{
    public interface ITableStorageSettings
    {
        string ConnectionString { get; set; }
        string TableName { get; set; }
    }
}
