using AzureStorageConsole.AzureBlobStorage;
using AzureStorageConsole.AzureQueueStorage;
using AzureStorageConsole.Services.Queues;
using AzureStorageConsole.Services.Storages;
using AzureStorageConsole.Services.Table.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageConsole
{
    class Program
    {

        public static IConfigurationRoot Configuration { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }

        static async Task Main(string[] args)
        {
            Initialize();

            await ListFiles();

            Console.WriteLine();
            Console.WriteLine();

            await EnviarMensagem();

            await ReceberMensagem();
        }

        static async Task EnviarMensagem()
        {
            bool continuar;
            do
            {
                Console.WriteLine("Digite Primeiro Nome:");
                string nome = Console.ReadLine();
                Console.WriteLine("Digite Seguindo Nome:");
                string segundoNome = Console.ReadLine();
                Console.WriteLine("Digite Telefone:");
                string telefone = Console.ReadLine();
                Console.WriteLine("Digite Data Nascimento:");
                DateTime dataNascimento = DateTime.Parse(Console.ReadLine());

                Customer customer = new Customer
                {
                    FirstName = nome,
                    LastName = segundoNome,
                    PhoneNumber = telefone,
                    DateOfBorn = dataNascimento
                };

                var queueSender = ServiceProvider.GetService<IQueueSenderService>();
                await queueSender.SendMessage(customer);
                Console.WriteLine("Deseja continuar? Digite S-(Sim) ou N-(Não)");
                continuar = Console.ReadLine() != "N";
                Console.Clear();
            }
            while (continuar);

        }

        static async Task ReceberMensagem()
        {
            bool continuar;
            do
            {
                var queueReceiver = ServiceProvider.GetService<IQueueReceiverService>();
                var received = await queueReceiver.Receive<Customer>(CancellationToken.None);
                if (received == default(IEnumerable<Customer>))
                    Console.WriteLine("Não foi recebido nenhum item da Fila tente novamente");
                else
                {
                    foreach (var value in received)
                        Console.WriteLine($"item recebido da fila {value.FirstName}- {value.LastName}, {value.PhoneNumber}. Data Nascimento: {value.DateOfBorn:dd/MM/yyyy}");
                }

                Console.WriteLine("Deseja tentar receber nova mensagem? Digite S-(Sim) ou N-(Não)");
                continuar = Console.ReadLine() != "N";
            }
            while (continuar);
        }


        static void Initialize()
        {

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }


        static async Task ListFiles()
        {
            var blobStorage = ServiceProvider.GetService<IBlobStorageService>();

            var files = await blobStorage.SearchFilesInContainer("");
            foreach (var file in files)
            {
                Console.WriteLine(file.FileName);
            }
        }

        static void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            var settings = Configuration.GetSection("RoomStoreFilesSettingsDebug").Get<AzureStorageSettings>();
            var queueSettings = Configuration.GetSection("RoomStorageQueueDebug").Get<AzureQueueSettings>();
#else
            var settings = Configuration.GetSection("RoomStoreFilesSettings").Get<AzureStorageSettings>();
            var queueSettings = Configuration.GetSection("RoomStorageQueue").Get<AzureQueueSettings>();
#endif
            services.AddScoped<IBlobStorageService, BlobStorageService>()
                .AddScoped<IQueueSenderService, AzureQueueSenderService>()
                .AddSingleton<IStorageSettings, AzureStorageSettings>(x=> settings)
                .AddSingleton<IQueueSettings, AzureQueueSettings>(x => queueSettings)
                .AddScoped<IQueueReceiverService, AzureQueueReceiverService>() ;

        }
    }
}
