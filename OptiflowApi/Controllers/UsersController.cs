using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using OptiflowApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OptiflowApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountb04;AccountKey=poKqldZvtuNZa9o7FZywImbwONmk7tSzmXm5wqO5kXY+NxjVSo7gFoSAbAS4UoJqVrqExpgHhcSWiVRIb8hmYg==";
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;

        public UsersController()
        {
            // Retrieve the storage account from the connection string
            storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "users" table
            table = tableClient.GetTableReference("users");

            // Create the table if it doesn't exist.
            CreateTable();
        }

        public async void CreateTable()
        {
            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = new List<User>();

            // Construct the query operation
            TableQuery<User> query = new TableQuery<User>();

            TableQuerySegment<User> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //add found person to list persons
            foreach (User foundUser in tableQueryResult)
            {
                users.Add(foundUser);
            }

            return Ok(users);
        }

        // GET api/users/byEmail/user@email.com
        [Route("api/users/byEmail/{email}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            int teller = 0;
            User user = new User();

            // Construct the query operation to find user
            TableQuery<User> query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            TableQuerySegment<User> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if user exists
            foreach (User foundUser in tableQueryResult)
            {
                teller++;
                user = foundUser;
            }

            if (teller > 0)
            {
                return Ok(user);
            }
            else
            {
                return NotFound(null);
            }
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            int teller = 0;
            User user = new User();

            // Construct the query operation to find user
            TableQuery<User> query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<User> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if user exists
            foreach (User foundUser in tableQueryResult)
            {
                teller++;
                user = foundUser;
            }

            if (teller > 0)
            {
                return Ok(user);
            }
            else
            {
                return NotFound(null);
            }
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]User user)
        {
            int tellerExists = 0;
            int teller = 0;
            int count = 0;
            long id = 0;
            User insertedUser = new User();

            // Construct the query operation to check if user with e-mail already exists
            TableQuery<User> queryExists = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, user.Email));

            TableQuerySegment<User> tableQueryResultExists = await table.ExecuteQuerySegmentedAsync(queryExists, null);

            //check if user exists
            foreach (User foundUser in tableQueryResultExists)
            {
                tellerExists++;
            }

            if (tellerExists > 0)
            {
                throw new Exception("User with e-mail already exists!");
            }
            else
            {
                // Construct the query operation to get all users
                TableQuery<User> queryAll = new TableQuery<User>();

                TableQuerySegment<User> tableQueryResultAll = await table.ExecuteQuerySegmentedAsync(queryAll, null);

                //get id of last user
                foreach (User foundUser in tableQueryResultAll)
                {
                    id = foundUser.Id;
                    count++;
                }

                if (count > 0)
                {
                    id++;
                }

                // Create a new user entity
                User newUser = new User(id, user.FirstName, user.LastName, user.Email, user.Password);
                newUser.Id = id;
                newUser.LastName = user.LastName;
                newUser.FirstName = user.FirstName;
                newUser.Email = user.Email;
                newUser.Password = user.Password;

                // Create the TableOperation object that inserts the user
                TableOperation insertOperation = TableOperation.Insert(newUser);

                // Execute the insert operation
                await table.ExecuteAsync(insertOperation);

                // Construct the query operation to find the new user
                TableQuery<User> query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, newUser.Email));

                TableQuerySegment<User> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

                foreach (User foundUser in tableQueryResult)
                {
                    teller++;
                    insertedUser = foundUser;
                }

                if (teller > 0)
                {
                    return Ok(insertedUser);
                }
                else
                {
                    return NotFound(null);
                }
            }
        }

        // PUT api/users/byEmail/user@email.com
        [Route("api/users/byEmail/{email}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserByEmail(string email, [FromBody]User user)
        {
            int teller = 0;
            bool exists = false;
            User userToUpdate = new User();

            // Construct the query operation to find user
            TableQuery<User> query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            TableQuerySegment<User> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if user exists
            foreach (User foundUser in tableQueryResult)
            {
                teller++;
                userToUpdate = foundUser;
            }

            if (teller > 0)
            {
                // Construct the query operation to check if user with the new provided e-mailaddress already exists
                TableQuery<User> queryExists = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, user.Email));

                TableQuerySegment<User> tableQueryResultExists = await table.ExecuteQuerySegmentedAsync(queryExists, null);

                //compare the id of the found user with the user you want to update
                foreach (User foundUser in tableQueryResultExists)
                {
                    if (foundUser.Id.Equals(userToUpdate.Id))
                    {
                        exists = false;
                    }
                    else
                    {
                        exists = true;
                    }
                }

                //throw exception if user with e-mail already exists
                if (exists)
                {
                    throw new Exception("User with e-mail already exists!");
                }
                else
                {
                    User updatedUser = userToUpdate;
                    updatedUser.LastName = user.LastName;
                    updatedUser.FirstName = user.FirstName;
                    updatedUser.Email = user.Email;
                    updatedUser.Password = user.Password;

                    // Create the TableOperation object that updates the user
                    TableOperation updateOperation = TableOperation.Replace(updatedUser);

                    // Execute the update operation
                    await table.ExecuteAsync(updateOperation);

                    return Ok(updatedUser);
                }
            }
            else
            {
                return NotFound(null);
            }
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserById(long id, [FromBody]User user)
        {
            int teller = 0;
            bool exists = false;
            User userToUpdate = new User();

            // Construct the query operation to find user
            TableQuery<User> query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<User> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if user exists
            foreach (User foundUser in tableQueryResult)
            {
                teller++;
                userToUpdate = foundUser;
            }

            if (teller > 0)
            {
                // Construct the query operation to check if user with the new provided e-mailaddress already exists
                TableQuery<User> queryExists = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, user.Email));

                TableQuerySegment<User> tableQueryResultExists = await table.ExecuteQuerySegmentedAsync(queryExists, null);

                //compare the id of the found user with the user you want to update
                foreach (User foundUser in tableQueryResultExists)
                {
                    if (foundUser.Id.Equals(userToUpdate.Id))
                    {
                        exists = false;
                    }
                    else
                    {
                        exists = true;
                    }
                }

                //throw exception if user with e-mail already exists
                if (exists)
                {
                    throw new Exception("User with e-mail already exists!");
                }
                else
                {
                    User updatedUser = userToUpdate;
                    updatedUser.LastName = user.LastName;
                    updatedUser.FirstName = user.FirstName;
                    updatedUser.Email = user.Email;
                    updatedUser.Password = user.Password;

                    // Create the TableOperation object that updates the user
                    TableOperation updateOperation = TableOperation.Replace(updatedUser);

                    // Execute the update operation
                    await table.ExecuteAsync(updateOperation);

                    return Ok(updatedUser);
                }
            }
            else
            {
                return NotFound(null);
            }
        }

        // DELETE api/users/byEmail/user@email.com
        [Route("api/users/byEmail/{email}")] 
        [HttpDelete]
        public async Task<IActionResult> DeleteUserByEmail(string email)
        {
            int teller = 0;
            User user = new User();

            // Construct the query operation
            TableQuery<User> query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            TableQuerySegment<User> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if user exists
            foreach (User foundUser in tableQueryResult)
            {
                teller++;
                user = foundUser;
            }

            if (teller > 0)
            {
                // Create the TableOperation object that deletes the user
                TableOperation deleteOperation = TableOperation.Delete(user);

                // Execute the delete operation
                await table.ExecuteAsync(deleteOperation);

                return Ok(user);
            }
            else
            {
                return NotFound(null);
            }
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(long id)
        {
            int teller = 0;
            User user = new User();

            // Construct the query operation
            TableQuery<User> query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<User> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if user exists
            foreach (User foundUser in tableQueryResult)
            {
                teller++;
                user = foundUser;
            }

            if (teller > 0)
            {
                // Create the TableOperation object that deletes the user
                TableOperation deleteOperation = TableOperation.Delete(user);

                // Execute the delete operation
                await table.ExecuteAsync(deleteOperation);

                return Ok(user);
            }
            else
            {
                return NotFound(null);
            }
        }
    }
}
