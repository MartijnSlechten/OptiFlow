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

        public async Task<Sensordata> FindDataRecord(long id)
        {
            int teller = 0;
            Sensordata dataRecord = new Sensordata();

            // Construct the query operation to find datarecord
            TableQuery<Sensordata> query = new TableQuery<Sensordata>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            TableQuerySegment<Sensordata> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //check if datarecord exists
            foreach (Sensordata foundData in tableQueryResult)
            {
                teller++;
                dataRecord = foundData;
            }

            if (teller > 0)
            {
                return dataRecord;
            }
            else
            {
                return null;
            }
        }

        public async Task<int> CountDataRecords()
        {
            int count = 0;
            List<Sensordata> data = await GetAllSensordata();

            foreach (Sensordata dataRecord in data)
            {
                count++;
            }

            return count;
        }

        public async Task<Sensordata> GetLastDataRecord()
        {
            Sensordata lastDataRecord = new Sensordata();
            int count = await CountDataRecords();

            if (count > 0)
            {
                List<Sensordata> data = await GetAllSensordata();

                //get last datarecord
                foreach (Sensordata dataRecord in data)
                {
                    lastDataRecord = dataRecord;
                }

                return lastDataRecord;
            }
            else
            {
                return null;
            }
        }

        // GET api/sensordata
        [HttpGet]
        public async Task<List<Sensordata>> GetAllSensordata()
        {
            List<Sensordata> data = new List<Sensordata>();

            // Construct the query operation
            TableQuery<Sensordata> query = new TableQuery<Sensordata>();

            TableQuerySegment<Sensordata> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            //add found record to list data
            foreach (Sensordata foundData in tableQueryResult)
            {
                data.Add(foundData);
            }

            return data;
        }

        // GET api/sensordata/5
        [HttpGet("{id}")]
        public async Task<Sensordata> GetDataRecord(long id)
        {
            return await FindDataRecord(id);
        }

        // POST api/sensordata
        [HttpPost]
        public async Task<Sensordata> CreateDataRecord([FromBody]Sensordata dataRecord)
        {
            long id = 0;

            //get last datarecord
            Sensordata lastDataRecord = await GetLastDataRecord();

            if (lastDataRecord != null)
            {
                id = lastDataRecord.Id + 1;
            }
            
            // Create a new sensordata entity
            Sensordata newRecord = new Sensordata(id, dataRecord.Time, dataRecord.Flow, dataRecord.Volume);
            newRecord.Id = id;
            newRecord.Time = dataRecord.Time;
            newRecord.Flow = dataRecord.Flow;
            newRecord.Volume = dataRecord.Volume;

            // Create the TableOperation object that inserts the datarecord
            TableOperation insertOperation = TableOperation.Insert(newRecord);

            // Execute the insert operation
            await table.ExecuteAsync(insertOperation);

            // return the inserted datarecord
            return await FindDataRecord(id);
        }

        // PUT api/sensordata/5
        [HttpPut("{id}")]
        public async Task<Sensordata> UpdateDataRecord(long id, [FromBody]Sensordata dataRecord)
        {
            //check if datarecord exists
            Sensordata dataToUpdate = await FindDataRecord(id);

            if (dataToUpdate != null)
            {
                Sensordata updatedData = dataToUpdate;
                updatedData.Time = dataRecord.Time;
                updatedData.Flow = dataRecord.Flow;
                updatedData.Volume = dataRecord.Volume;

                // Create the TableOperation object that updates the user
                TableOperation updateOperation = TableOperation.Replace(updatedData);

                // Execute the update operation
                await table.ExecuteAsync(updateOperation);

                return updatedData;
            }
            else
            {
                return null;
            }
        }

        // DELETE api/sensordata/5
        [HttpDelete("{id}")]
        public async Task<Sensordata> DeleteData(long id)
        {
            //check if datarecord exists
            Sensordata dataRecord = await FindDataRecord(id);

            if (dataRecord != null)
            {
                // Create the TableOperation object that deletes the user
                TableOperation deleteOperation = TableOperation.Delete(dataRecord);

                // Execute the delete operation
                await table.ExecuteAsync(deleteOperation);

                return dataRecord;
            }
            else
            {
                return null;
            }
        }
    }
}
