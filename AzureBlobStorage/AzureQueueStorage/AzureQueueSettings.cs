using AzureStorageConsole.Services.Queues;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.AzureQueueStorage
{
    public class AzureQueueSettings : IQueueSettings
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
        public int ExpireMessageInDays { get; set; }
    }
}
