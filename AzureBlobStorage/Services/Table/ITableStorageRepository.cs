using AzureStorageConsole.Services.Table;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageConsole.Blobs.Services.Table
{
    public interface ITableStorageRepository<T> where T : new()
    {
        Task<T> InsertOrUpdate(T data);

        Task Delete(T data);

        Task<T> Find(object id);

        Task<IEnumerable<T>> GetAll();

        Task<IEnumerable<T>> Get(TableStorageConditionCollection condition);

        Task ChangeSettings(ITableStorageSettings settings);
    }
}
