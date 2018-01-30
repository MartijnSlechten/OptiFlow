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
    public class SensordataController : Controller
    {
        private string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountb04;AccountKey=poKqldZvtuNZa9o7FZywImbwONmk7tSzmXm5wqO5kXY+NxjVSo7gFoSAbAS4UoJqVrqExpgHhcSWiVRIb8hmYg==";
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;

        public SensordataController()
        {
            // Retrieve the storage account from the connection string
            storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "sensordata" table
            table = tableClient.GetTableReference("sensordata");

            // Create the table if it doesn't exist.
            CreateTable();
        }

        public async void CreateTable()
        {
            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();
        }

        // GET api/sensordata
        [HttpGet]
        public async Task<IActionResult> GetSensordata()
        {
            List<Sensordata> data = new List<Sensordata>();

            // Construct the query operation
            TableQuery<Sensordata> query = new TableQuery<Sensordata>();

            TableQuerySegment<Sensordata> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //add found person to list persons
            foreach (Sensordata foundData in tableQueryResult)
            {
                data.Add(foundData);
            }

            return Ok(data);
        }

        // GET api/sensordata/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataRecord(long id)
        {
            int teller = 0;
            Sensordata dataRecord = new Sensordata();

            // Construct the query operation to find user
            TableQuery<Sensordata> query = new TableQuery<Sensordata>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<Sensordata> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if user exists
            foreach (Sensordata foundData in tableQueryResult)
            {
                teller++;
                dataRecord = foundData;
            }

            if (teller > 0)
            {
                return Ok(dataRecord);
            }
            else
            {
                return NotFound(null);
            }
        }

        // POST api/sensordata
        [HttpPost]
        public async Task<IActionResult> CreateData([FromBody]Sensordata dataRecord)
        {
            int teller = 0;
            long id = 0;
            Sensordata insertedData = new Sensordata();

            // Construct the query operation to get all sensordata
            TableQuery<Sensordata> queryAll = new TableQuery<Sensordata>();

            TableQuerySegment<Sensordata> tableQueryResultAll = await table.ExecuteQuerySegmentedAsync(queryAll, null);

            //get id of last user
            foreach (Sensordata foundData in tableQueryResultAll)
            {
                id = foundData.Id;
            }
            
            // Create a new customer entity.
            Sensordata newRecord = new Sensordata(id, dataRecord.Tijd, dataRecord.Flow, dataRecord.Volume);
            newRecord.Id = id;
            newRecord.Tijd = dataRecord.Tijd;
            newRecord.Flow = dataRecord.Flow;
            newRecord.Volume = dataRecord.Volume;

            // Create the TableOperation object that inserts the user
            TableOperation insertOperation = TableOperation.Insert(newRecord);

            // Execute the insert operation
            await table.ExecuteAsync(insertOperation);

            // Construct the query operation to find the new user
            TableQuery<Sensordata> query = new TableQuery<Sensordata>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<Sensordata> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            foreach (Sensordata foundData in tableQueryResult)
            {
                teller++;
                insertedData = foundData;
            }

            if (teller > 0)
            {
                return Ok(insertedData);
            }
            else
            {
                return NotFound(null);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
