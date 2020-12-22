using AzureStorageConsole.Blobs.Services.Table;


namespace AzureStorageConsole.AzureBlobStorage
{
    public class AzureTableStorageSettings : ITableStorageSettings
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
    }
}
