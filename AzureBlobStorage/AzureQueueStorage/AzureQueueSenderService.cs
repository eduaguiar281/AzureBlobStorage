using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using AzureStorageConsole.Services.Queues;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureStorageConsole.AzureQueueStorage
{
    public class AzureQueueSenderService : IQueueSenderService
    {
        protected QueueClient _client;

        public AzureQueueSenderService(IQueueSettings settings)
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

        public async Task SendMessage<T>(T message)
        {
            string stringMessage = JsonSerializer.Serialize(message);
            Response<SendReceipt> responseRecipient;
            if (Settings.ExpireMessageInDays == 0)
                responseRecipient = await _client.SendMessageAsync(stringMessage, default, TimeSpan.FromSeconds(-1), default);
            else
                responseRecipient = await _client.SendMessageAsync(stringMessage, default, TimeSpan.FromDays(Settings.ExpireMessageInDays), default);

             

        }
    }
}
