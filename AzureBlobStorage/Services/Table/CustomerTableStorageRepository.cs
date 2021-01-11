using AzureStorageConsole.AzureTableStorage;
using AzureStorageConsole.Services.Table;
using AzureStorageConsole.Services.Table.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageConsole.Blobs.Services.Table
{
    public class CustomerTableStorageRepository : ITableStorageRepository<Customer> 
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _cloudTable;

        private readonly TableSet<Customer> _tableSet;

        public CustomerTableStorageRepository(ConsoleDataContext dataContext)
        {
            _tableSet = dataContext.Set<Customer>();
        }

        public async Task Delete(Customer data)
        {
            await _tableSet.Delete(data.Id);
        }

        public async Task<IEnumerable<T>> Get<T>(Func<T, bool> predicate) where T : ITableEntity, new()
        {
            TableQuery<T> linqQuery = _cloudTable.CreateQuery<T>();

            linqQuery.Where(predicate);

            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {

                TableQuerySegment<T> seg = await _cloudTable.ExecuteQuerySegmentedAsync<T>(linqQuery, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);

            } while (token != null);

            return items;
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : ITableEntity, new ()
        {
            TableQuery<T> linqQuery = _cloudTable.CreateQuery<T>();

            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {

                TableQuerySegment<T> seg = await _cloudTable.ExecuteQuerySegmentedAsync<T>(linqQuery, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);

            } while (token != null);

            return items;
        }

        public async Task<T> GetSingle<T>(string partitionKey, string rowKey) where T : ITableEntity, new ()
        {

            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                TableResult result = await _cloudTable.ExecuteAsync(retrieveOperation);
                T customer = (T)result.Result;

                return customer;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<T> InsertOrUpdate<T>(T data) where T : ITableEntity
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            try
            {
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(data);

                TableResult result = await _cloudTable.ExecuteAsync(insertOrMergeOperation);
                T insertedCustomer = (T)result.Result;

                return insertedCustomer;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task ChangeSettings(ITableStorageSettings settings)
        {
            CreateStorageAccountFromConnectionString(settings.ConnectionString);
            await CreateTableAsync(settings.TableName);
        }


        private async Task CreateTableAsync(string tableName)
        {
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            _cloudTable = tableClient.GetTableReference(tableName);
            if (await _cloudTable.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                Console.WriteLine("Table {0} already exists", tableName);
            }

            Console.WriteLine();
        }

        private void CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            try
            {
                _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Informações de conta de armazenamento fornecidas não é valida. Confirme se AccountName e AccountKey são válidos no arquivo appsettings.json - depois reinicie o aplicativo.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Informações de conta de armazenamento fornecidas não é valida. Confirme se AccountName e AccountKey são válidos no arquivo appsettings.json - em seguida, reinicie o exemplo.");
                throw;
            }
        }

        public Task<Customer> InsertOrUpdate(Customer data)
        {
            throw new NotImplementedException();
        }

        public Task<Customer> Find(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> Get(TableStorageConditionCollection condition)
        {
            throw new NotImplementedException();
        }
    }
}
