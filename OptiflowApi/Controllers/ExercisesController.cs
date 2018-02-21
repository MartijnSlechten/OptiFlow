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
    public class ExercisesController : Controller
    {
        private string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountb04;AccountKey=poKqldZvtuNZa9o7FZywImbwONmk7tSzmXm5wqO5kXY+NxjVSo7gFoSAbAS4UoJqVrqExpgHhcSWiVRIb8hmYg==";
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;

        public ExercisesController()
        {
            // Retrieve the storage account from the connection string
            storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "exercises" table
            table = tableClient.GetTableReference("exercises");

            // Create the table if it doesn't exist.
            CreateTable();
        }

        public ExercisesController(String tablename)
        {
            // Retrieve the storage account from the connection string
            storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "users" table
            table = tableClient.GetTableReference(tablename);

            // Create the table if it doesn't exist.
            CreateTable();
        }

        public async void CreateTable()
        {
            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();
        }

        public async Task<Exercise> FindExercise(long id)
        {
            int teller = 0;
            Exercise exercise = new Exercise();

            // Construct the query operation to find exercise
            TableQuery<Exercise> query = new TableQuery<Exercise>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<Exercise> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if exercise exists
            foreach (Exercise foundExercise in tableQueryResult)
            {
                teller++;
                exercise = foundExercise;
            }

            if (teller > 0)
            {
                return exercise;
            }
            else
            {
                return null;
            }
        }

        public async Task<int> CountExercises()
        {
            int count = 0;
            List<Exercise> exercises = await GetExercises();

            foreach (Exercise exercise in exercises)
            {
                count++;
            }

            return count;
        }

        public async Task<Exercise> GetLastExercise()
        {
            Exercise lastExercise = new Exercise();
            int count = await CountExercises();

            if (count > 0)
            {
                List<Exercise> exercises = await GetExercises();

                //get last exercise
                foreach (Exercise exercise in exercises)
                {
                    lastExercise = exercise;
                }

                return lastExercise;
            }
            else
            {
                return null;
            }
        }

        // GET api/exercises
        [HttpGet]
        public async Task<List<Exercise>> GetExercises()
        {
            List<Exercise> exercises = new List<Exercise>();

            // Construct the query operation
            TableQuery<Exercise> query = new TableQuery<Exercise>();

            TableQuerySegment<Exercise> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //add found exercise to list exercises
            foreach (Exercise foundExercise in tableQueryResult)
            {
                exercises.Add(foundExercise);
            }

            return exercises;
        }

        // GET api/exercises/5
        [HttpGet("{id}")]
        public async Task<Exercise> GetExercise(long id)
        {
            return await FindExercise(id);
        }

        // POST api/exercises
        [HttpPost]
        public async Task<Exercise> CreateExercise([FromBody]Exercise exercise)
        {
            long id = 0;

            //get last exercise
            Exercise lastExercise = await GetLastExercise();

            if (lastExercise != null)
            {
                id = lastExercise.Id + 1;
            }

            // Create a new exercise entity
            Exercise newExercise = new Exercise(id, exercise.Name, exercise.Description, exercise.ImageSource);
            newExercise.Id = id;
            newExercise.Name = exercise.Name;
            newExercise.Description = exercise.Description;
            newExercise.ImageSource = exercise.ImageSource;

            // Create the TableOperation object that inserts the datarecord
            TableOperation insertOperation = TableOperation.Insert(newExercise);

            // Execute the insert operation
            await table.ExecuteAsync(insertOperation);

            // return the inserted exercise
            return await FindExercise(id);
        }

        // PUT api/exercises/5
        [HttpPut("{id}")]
        public async Task<Exercise> UpdateExercise(long id, [FromBody]Exercise exercise)
        {
            //check if exercise exists
            Exercise exerciseToUpdate = await FindExercise(id);

            if (exerciseToUpdate != null)
            {
                Exercise updatedExercise = exerciseToUpdate;
                updatedExercise.Name = exercise.Name;
                updatedExercise.Description = exercise.Description;
                updatedExercise.ImageSource = exercise.ImageSource;

                // Create the TableOperation object that updates the user
                TableOperation updateOperation = TableOperation.Replace(updatedExercise);

                // Execute the update operation
                await table.ExecuteAsync(updateOperation);

                return updatedExercise;
            }
            else
            {
                return null;
            }
        }

        // DELETE api/exercises/5
        [HttpDelete("{id}")]
        public async Task<Exercise> DeleteExercise(long id)
        {
            //check if exercise exists
            Exercise exercise = await FindExercise(id);

            if (exercise != null)
            {
                // Create the TableOperation object that deletes the user
                TableOperation deleteOperation = TableOperation.Delete(exercise);

                // Execute the delete operation
                await table.ExecuteAsync(deleteOperation);

                return exercise;
            }
            else
            {
                return null;
            }
        }
    }
}
