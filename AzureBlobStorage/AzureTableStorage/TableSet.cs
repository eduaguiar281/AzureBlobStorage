using AzureStorageConsole.Services.Table;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageConsole.AzureTableStorage
{
    public class TableSet<TEntity> where TEntity :class, new()
    {
        
        private string partitionKey;
        private string tableName;
        private string connectionString;

        internal CloudTableClient tableClient;
        internal CloudTable table;

        public TableSet(string connectionString, string tableName)
        {
            this.partitionKey = typeof(TEntity).Name;
            this.tableName = tableName;
            this.connectionString = connectionString;

            //pluralise the partition key (because basically it is the 'table' name).
            if (partitionKey.Substring(partitionKey.Length - 1, 1).ToLower() == "y")
                partitionKey = partitionKey.Substring(0, partitionKey.Length - 1) + "ies";

            if (partitionKey.Substring(partitionKey.Length - 1, 1).ToLower() != "s")
                partitionKey = partitionKey + "s";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
        }

        public virtual TEntity Find(object id)
        {
            var query = new TableQuery().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
            var dto = table.ExecuteQuery(query).First();
            TEntity mapped = StripDTO(dto);

            return mapped;
        }

        public virtual async Task Delete(object id)
        {
            var query = new TableQuery().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
            var dto = table.ExecuteQuery(query).First();
            TableOperation deleteOperation = TableOperation.Delete(dto);
            //TableResult result =
            await table.ExecuteAsync(deleteOperation);
        }

        public virtual List<TEntity> Get(TableStorageConditionCollection conditions)
        {
            if (conditions == null)
                throw new ArgumentNullException(nameof(conditions));

            List<TEntity> mappedList = new List<TEntity>();
            var query = new TableQuery().Where(conditions.CombineConditions()); //get all customers - because Customer is our partition key
            var result = table.ExecuteQuery(query).ToList();
            foreach (var item in result)
            {
                mappedList.Add(StripDTO(item));
            }
            return mappedList;
        }


        public virtual List<TEntity> GetAll()
        {
            List<TEntity> mappedList = new List<TEntity>();
            var query = new TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey)); //get all customers - because Customer is our partition key
            var result = table.ExecuteQuery(query).ToList();

            foreach (var item in result)
            {
                mappedList.Add(StripDTO(item));
            }
            return mappedList;
        }

        public virtual void InsertOrUpdate(TEntity entity)
        {
            dynamic mapped = CreateDTO(entity);
            TableOperation insertOperation = TableOperation.InsertOrMerge(mapped);
            table.Execute(insertOperation);
        }


        private dynamic CreateDTO(object a)
        {
            TableEntityDTO dto = new TableEntityDTO();
            object rowKey = null;

            Type t1 = a.GetType();
            Type t2 = dto.GetType();

            //now set all the entity properties
            foreach (PropertyInfo p in t1.GetProperties())
            {
                dto.TrySetMember(p.Name, p.GetValue(a, null) == null ? "" : p.GetValue(a, null));
                if (IsId(p.Name))
                    rowKey = p.GetValue(a, null);
            }

            if (rowKey == null)
                rowKey = Guid.NewGuid();

            dto.RowKey = rowKey.ToString();
            dto.PartitionKey = partitionKey;

            return dto;
        }

        private object GetValue(EntityProperty source)
        {
            switch (source.PropertyType)
            {
                case EdmType.Binary:
                    return (object)source.BinaryValue;
                case EdmType.Boolean:
                    return (object)source.BooleanValue;
                case EdmType.DateTime:
                    return (object)source.DateTimeOffsetValue;
                case EdmType.Double:
                    return (object)source.DoubleValue;
                case EdmType.Guid:
                    return (object)source.GuidValue;
                case EdmType.Int32:
                    return (object)source.Int32Value;
                case EdmType.Int64:
                    return (object)source.Int64Value;
                case EdmType.String:
                    return (object)source.StringValue;
                default: throw new TypeLoadException(string.Format("not supported edmType:{0}", source.PropertyType));
            }
        }

        private TEntity StripDTO (DynamicTableEntity a)
        {
            return StripDTO(a.Properties);
        }

        private TEntity StripDTO(IDictionary<string, EntityProperty> dictionary)
        {
            TEntity result = new TEntity();

            Type t1 = result.GetType();

            foreach (PropertyInfo p1 in t1.GetProperties())//for each property in the entity,
            {
                foreach (var value in dictionary)//see if we have a correspinding property in the DTO
                {
                    if (p1.Name == value.Key)
                    {
                        p1.SetValue(result, GetValue(value.Value));
                    }
                }

            }

            return result;
        }

        private bool IsId(string candidate)
        {
            bool result = false;

            if (candidate.ToLower() == "id")
                result = true;

            if (candidate.ToLower().Substring(candidate.Length - 2, 2) == "id")
                result = true;

            return result;
        }
    }
}

//https://bretthargreaves.wordpress.com/2014/01/11/azure-table-storage-with-repository-pattern/
