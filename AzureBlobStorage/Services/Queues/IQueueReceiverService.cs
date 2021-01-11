using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageConsole.Services.Queues
{
    public interface IQueueReceiverService
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
        /// Faz a leitura dos itens da fila
        /// </summary>
        /// <typeparam name="T">Retorno Genérico</typeparam>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Receive<T>(CancellationToken cancellationToken);




    }
}
