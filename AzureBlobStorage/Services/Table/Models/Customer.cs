using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;


namespace AzureStorageConsole.Services.Table.Models
{
    public class Customer: Entity<Guid> 
    {
        public Customer()
        {
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBorn { get; set; } 
    }
}
