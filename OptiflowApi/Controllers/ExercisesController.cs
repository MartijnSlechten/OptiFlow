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

        public async void CreateTable()
        {
            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();
        }

        // GET api/exercises
        [HttpGet]
        public async Task<IActionResult> GetExercises()
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

            return Ok(exercises);
        }

        // GET api/exercises/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExercise(long id)
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
                return Ok(exercise);
            }
            else
            {
                return NotFound(null);
            }
        }

        // POST api/exercises
        [HttpPost]
        public async Task<IActionResult> CreateExercise([FromBody]Exercise exercise)
        {
            int teller = 0;
            int count = 0;
            long id = 0;
            Exercise insertedExercise = new Exercise();

            // Construct the query operation to get all exercises
            TableQuery<Exercise> queryAll = new TableQuery<Exercise>();

            TableQuerySegment<Exercise> tableQueryResultAll = await table.ExecuteQuerySegmentedAsync(queryAll, null);

            //get id of last user
            foreach (Exercise foundExercise in tableQueryResultAll)
            {
                id = foundExercise.Id;
                count++;
            }

            if (count > 0)
            {
                id++;
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

            // Construct the query operation to find the new datarecord
            TableQuery<Exercise> query = new TableQuery<Exercise>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<Exercise> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            foreach (Exercise foundExercise in tableQueryResult)
            {
                teller++;
                insertedExercise = foundExercise;
            }

            if (teller > 0)
            {
                return Ok(insertedExercise);
            }
            else
            {
                return NotFound(null);
            }
        }

        // PUT api/exercises/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExercise(long id, [FromBody]Exercise exercise)
        {
            int teller = 0;
            Exercise exerciseToUpdate = new Exercise();

            // Construct the query operation to find datarecord
            TableQuery<Exercise> query = new TableQuery<Exercise>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<Exercise> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if datarecord exists
            foreach (Exercise foundExercise in tableQueryResult)
            {
                teller++;
                exerciseToUpdate = foundExercise;
            }

            if (teller > 0)
            {
                Exercise updatedExercise = exerciseToUpdate;
                updatedExercise.Name = exercise.Name;
                updatedExercise.Description = exercise.Description;
                updatedExercise.ImageSource = exercise.ImageSource;

                // Create the TableOperation object that updates the user
                TableOperation updateOperation = TableOperation.Replace(updatedExercise);

                // Execute the update operation
                await table.ExecuteAsync(updateOperation);

                return Ok(updatedExercise);
            }
            else
            {
                return NotFound(null);
            }
        }

        // DELETE api/exercises/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(long id)
        {
            int teller = 0;
            Exercise exercise = new Exercise();

            // Construct the query operation
            TableQuery<Exercise> query = new TableQuery<Exercise>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<Exercise> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if user exists
            foreach (Exercise foundExercise in tableQueryResult)
            {
                teller++;
                exercise = foundExercise;
            }

            if (teller > 0)
            {
                // Create the TableOperation object that deletes the user
                TableOperation deleteOperation = TableOperation.Delete(exercise);

                // Execute the delete operation
                await table.ExecuteAsync(deleteOperation);

                return Ok(exercise);
            }
            else
            {
                return NotFound(null);
            }
        }
    }
}
