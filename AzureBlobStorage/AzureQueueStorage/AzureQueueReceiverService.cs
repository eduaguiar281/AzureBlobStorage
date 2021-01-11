using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using AzureStorageConsole.Services.Queues;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageConsole.AzureQueueStorage
{
    public class AzureQueueReceiverService : IQueueReceiverService
    {

        protected QueueClient _client;

        public AzureQueueReceiverService(IQueueSettings settings)
        {
            Configure(settings).Wait();
        }

        public IQueueSettings Settings { get; private set; }


        public async Task Configure(IQueueSettings settings)
        {
            Settings = settings;
            _client = new QueueClient(Settings.ConnectionString, Settings.QueueName);

            Response queue = await _client.CreateIfNotExistsAsync();
            if (queue != null)
            {
                Console.WriteLine($"Fila {Settings.QueueName} foi criada");
            }
        }

        public async Task<IEnumerable<T>> Receive<T>(CancellationToken cancellationToken)
        {
            Response<QueueMessage[]> response = await _client.ReceiveMessagesAsync(Settings.PeekMaxMessages, null,  cancellationToken);

            if (response.Value == null)
                return default(IEnumerable<T>);

            List<T> result = new List<T>();
            foreach(QueueMessage item in response.Value)
            {
                result.Add(JsonSerializer.Deserialize<T>(item.MessageText));
                await _client.DeleteMessageAsync(item.MessageId, item.PopReceipt, cancellationToken);
            }


            return result; 
        }
    }
}
