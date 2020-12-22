using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.Services.Table.Models
{
    public class Entity<T> where T:struct
    {
        public T Id { get; set; }
    }
}
