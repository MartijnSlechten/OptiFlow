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

        public async Task<User> FindUserById(long id)
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
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<User> FindUserByEmail(string email)
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
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<int> CountUsers()
        {
            int count = 0;
            List<User> users = await GetUsers();

            foreach (User user in users)
            {
                count++;
            }

            return count;
        }

        public async Task<User> GetLastUser()
        {
            User lastUser = new User();
            int count = await CountUsers();

            if (count > 0)
            {
                List<User> users = await GetUsers();

                //get last user
                foreach (User user in users)
                {
                    lastUser = user;
                }

                return lastUser;
            }
            else
            {
                return null;
            }
        }


        public async Task<bool> AuthenticateUser(string email, string password)
        {
            User foundUser = await GetUserByEmail(email);

            if (foundUser.Password.Equals(password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // GET api/users
        [HttpGet]
        public async Task<List<User>> GetUsers()
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

            return users;
        }

        // GET api/users/byEmail/user@email.com
        [Route("api/users/byEmail/{email}")]
        [HttpGet]
        public async Task<User> GetUserByEmail(string email)
        {
            return await FindUserByEmail(email);
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<User> GetUserById(long id)
        {
            return await FindUserById(id); ;
        }

        // POST api/users
        [HttpPost]
        public async Task<User> CreateUser([FromBody]User user)
        {
            long id = 0;

            //check is user with provided e-mailadress already exists
            User foundUser = await FindUserByEmail(user.Email);

            if (foundUser != null)
            {
                throw new Exception("User with e-mail already exists!");
            }
            else
            {
                //get last user
                User lastUser = await GetLastUser();
                
                if (lastUser != null)
                {
                    id = lastUser.Id + 1;
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

                //return the inserted user
                return await FindUserByEmail(newUser.Email);
            }
        }

        // PUT api/users/byEmail/user@email.com
        [Route("api/users/byEmail/{email}")]
        [HttpPut]
        public async Task<User> UpdateUserByEmail(string email, [FromBody]User user)
        {
            bool exists = false;

            //check if user exists
            User userToUpdate = await FindUserByEmail(email);

            if (userToUpdate != null)
            {
                // Construct the query operation to check if user with the new provided e-mailaddress already exists
                User foundUser = await FindUserByEmail(user.Email);
                
                if (foundUser.Id.Equals(userToUpdate.Id))
                {
                    exists = false;
                }
                else
                {
                    exists = true;
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

                    return updatedUser;
                }
            }
            else
            {
                return null;
            }
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public async Task<User> UpdateUserById(long id, [FromBody]User user)
        {
            bool exists = false;

            //check if user exists
            User userToUpdate = await FindUserById(id);

            if (userToUpdate != null)
            {
                // Construct the query operation to check if user with the new provided e-mailaddress already exists
                User foundUser = await FindUserByEmail(user.Email);

                if (foundUser.Id.Equals(userToUpdate.Id))
                {
                    exists = false;
                }
                else
                {
                    exists = true;
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

                    return updatedUser;
                }
            }
            else
            {
                return null;
            }
        }

        // DELETE api/users/byEmail/user@email.com
        [Route("api/users/byEmail/{email}")] 
        [HttpDelete]
        public async Task<User> DeleteUserByEmail(string email)
        {
            //check if user exists
            User user = await FindUserByEmail(email);

            if (user != null)
            {
                // Create the TableOperation object that deletes the user
                TableOperation deleteOperation = TableOperation.Delete(user);

                // Execute the delete operation
                await table.ExecuteAsync(deleteOperation);

                return user;
            }
            else
            {
                return null;
            }
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<User> DeleteUserById(long id)
        {
            //check if user exists
            User user = await FindUserById(id);

            if (user != null)
            {
                // Create the TableOperation object that deletes the user
                TableOperation deleteOperation = TableOperation.Delete(user);

                // Execute the delete operation
                await table.ExecuteAsync(deleteOperation);

                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
