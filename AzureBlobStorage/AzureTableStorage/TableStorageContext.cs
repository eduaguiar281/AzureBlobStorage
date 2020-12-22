using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.AzureTableStorage
{
    public abstract class TableStorageContext
    {
        private string tableName { get; set; }
        private string connectionString { get; set; }

        public TableStorageContext(string connectionString, string tableName)
        {
            this.tableName = tableName;
            this.connectionString = connectionString;
        }

        public virtual TableSet<TEntity> Set<TEntity>()
            where TEntity : class, new()
        {
            var set = new TableSet<TEntity>(connectionString, tableName);

            return set;
        }
    }
}
