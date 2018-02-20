using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApi.Models
{
    public class User : TableEntity
    {
        public User(long id)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = id.ToString();
            this.Id = id;
        }

        public User(long id, string firstName, string lastName, string email, string password)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = id.ToString();
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Password = password;
        }

        public User() { }

        public long Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
