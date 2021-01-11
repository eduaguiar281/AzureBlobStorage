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

        /// <summary>
        /// Define o número máximo de itens da fila a serem lidos
        /// </summary>
        int PeekMaxMessages { get; set; }
    }
}
