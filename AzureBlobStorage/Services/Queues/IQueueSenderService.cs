using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageConsole.Services.Queues
{
    public interface IQueueSenderService
    {
        /// <summary>
        /// Possui as configurações da Queue
        /// </summary>
        IQueueSettings Settings { get; }
        

        /// <summary>
        /// Define as configurações da Queue. Se a queue não existir é criado uma nova queue
        /// </summary>
        /// <param name="settings">Configurações da Fila</param>
        Task Configure(IQueueSettings settings);


        /// <summary>
        /// Envia mensagem para fila
        /// </summary>
        /// <typeparam name="T">Tipo genérico</typeparam>
        /// <param name="message">Mensagem a ser enviada para fila</param>
        Task SendMessage<T>(T message);

        
    }
}
