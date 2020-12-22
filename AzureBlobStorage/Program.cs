using AzureStorageConsole.AzureBlobStorage;
using AzureStorageConsole.AzureQueueStorage;
using AzureStorageConsole.Services.Queues;
using AzureStorageConsole.Services.Storages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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
        }

        static async Task EnviarMensagem()
        {
            bool continuar;
            do
            {
                Console.WriteLine("Digite sua mensagem:");
                var mensagem = Console.ReadLine();
                var queueSender = ServiceProvider.GetService<IQueueSenderService>();
                await queueSender.SendMessage(new { Client = "ConsoleApp01", Mensagem = mensagem });
                Console.WriteLine("Deseja continuar? Digite S-(Sim) ou N-(Não)");
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
                .AddSingleton<IQueueSettings, AzureQueueSettings>(x => queueSettings);

        }
    }
}
