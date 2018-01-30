using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OptiflowApi.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

namespace OptiflowApi.Controllers
{
    [Route("api/[controller]")]
    public class PersonController : Controller
    {
        private string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=webapistorageb4martijn;AccountKey=zQ3KIYCnGBtXh/WsP97pXZpmdo9A7ZrkUwDBYIEY+XIs2I5DQHrsZRMwDQVakFzyjWe+jdMQQMePPXUfrtq/uw==";

        // GET api/person
        [HttpGet]
        public async Task<IActionResult> GetPersons()
        {
            List<Person> persons = new List<Person>();

            // Retrieve the storage account from the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "testpeople" table
            CloudTable table = tableClient.GetTableReference("testpeople");

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();

            // Construct the query operation
            TableQuery<Person> query = new TableQuery<Person>();

            TableQuerySegment<Person> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //add found person to list persons
            foreach (Person foundPerson in tableQueryResult)
            {
                persons.Add(foundPerson);
            }

            return Ok(persons);
        }

        // GET api/person/5
        [HttpGet("{email}")]
        public async Task<IActionResult> GetPerson(string email)
        {
            int teller = 0;
            Person person = new Person();

            // Retrieve the storage account from the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "testpeople" table
            CloudTable table = tableClient.GetTableReference("testpeople");

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();

            // Construct the query operation
            TableQuery<Person> query = new TableQuery<Person>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            TableQuerySegment<Person> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            foreach (Person foundPerson in tableQueryResult)
            {
                teller++;
                person = foundPerson;
            }

            if (teller > 0)
            {
                return Ok(person);
            }
            else
            {
                return NotFound();
            }            
        }

        // POST api/person
        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody]Person person)
        {
            int teller = 0;
            Person insertedPerson = new Person();

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "testpeople" table.
            CloudTable table = tableClient.GetTableReference("testpeople");

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();

            // Create a new customer entity.
            Person newPerson = new Person(person.FirstName, person.LastName, person.Email);
            newPerson.LastName = person.LastName;
            newPerson.FirstName = person.FirstName;
            newPerson.Email = person.Email;

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(newPerson);

            // Execute the insert operation.
            await table.ExecuteAsync(insertOperation);

            // Construct the query operation
            TableQuery<Person> query = new TableQuery<Person>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, newPerson.Email));

            TableQuerySegment<Person> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            foreach (Person foundPerson in tableQueryResult)
            {
                teller++;
                insertedPerson = foundPerson;
            }

            if (teller > 0)
            {
                return Ok(insertedPerson);
            }
            else
            {
                return NotFound();
            }
        }

        // PUT api/person/5
        [HttpPut("{email}")]
        public async Task<IActionResult> UpdatePerson(string email, [FromBody]Person person)
        {
            int teller = 0;
            Person personToUpdate = new Person();

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "testpeople" table.
            CloudTable table = tableClient.GetTableReference("testpeople");

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();

            // Construct the query operation
            TableQuery<Person> query = new TableQuery<Person>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            TableQuerySegment<Person> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            foreach (Person foundPerson in tableQueryResult)
            {
                teller++;
                personToUpdate = foundPerson;
            }

            if (teller > 0)
            {
                Person updatedPerson = personToUpdate;
                updatedPerson.LastName = person.LastName;
                updatedPerson.FirstName = person.FirstName;
                updatedPerson.Email = person.Email;

                // Create the TableOperation object that inserts the customer entity.
                TableOperation updateOperation = TableOperation.Replace(updatedPerson);

                // Execute the insert operation.
                await table.ExecuteAsync(updateOperation);

                return Ok(updatedPerson);
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE api/person/5
        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            int teller = 0;
            Person person = new Person();

            // Retrieve the storage account from the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "testpeople" table
            CloudTable table = tableClient.GetTableReference("testpeople");

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();

            // Construct the query operation
            TableQuery<Person> query = new TableQuery<Person>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            TableQuerySegment<Person> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            foreach (Person foundPerson in tableQueryResult)
            {
                teller++;
                person = foundPerson;
            }

            if (teller > 0)
            {
                // Create the TableOperation object that inserts the customer entity.
                TableOperation deleteOperation = TableOperation.Delete(person);

                // Execute the insert operation.
                await table.ExecuteAsync(deleteOperation);

                return Ok(person);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
