using Microsoft.WindowsAzure.Storage.Table;

namespace OptiflowApi.Models
{
    public class Person : TableEntity
    {
        public Person(string email)
        {
            this.PartitionKey = email;
            this.RowKey = email;
            this.Email = email;
        }

        public Person(string firstName, string lastName, string email)
        {
            this.PartitionKey = email;
            this.RowKey = email;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
        }

        public Person() { }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }
    }
}
