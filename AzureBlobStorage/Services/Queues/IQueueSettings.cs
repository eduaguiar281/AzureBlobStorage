using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.Services.Queues
{
    public interface IQueueSettings
    {
        string ConnectionString { get; set; }
        string QueueName { get; set; }
        int ExpireMessageInDays { get; set; }
    }
}
