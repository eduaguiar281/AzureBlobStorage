using System;
using System.Collections.Generic;
using System.Text;
using AzureStorageConsole.Services.Table.Models;

namespace AzureStorageConsole.AzureTableStorage
{
    public class ConsoleDataContext : TableStorageContext
    {
        private static string tableName;
        private static string connection;

        public ConsoleDataContext(string connectionString, string table)
            : base(connectionString, table)
        {
            tableName = table;
            connection = connectionString;
        }

        public TableSet<Customer> Customers { get; set; }
    }
}
