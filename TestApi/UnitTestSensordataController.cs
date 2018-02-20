using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptiflowApi.Controllers;
using OptiflowApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestApi
{
    [TestClass]
    public class UnitTestSensordataController
    {
        SensordataController sensordataController = new SensordataController("sensordatatest");
        Sensordata dataRecord = new Sensordata(0, new DateTime(2017, 11, 15, 6, 55, 0), 0.250, 2.523);
        Sensordata dataRecord2 = new Sensordata(1, new DateTime(2018, 2, 6, 17, 30, 0), 3.252, 1.459);
        Sensordata dataRecord3 = new Sensordata(0, new DateTime(2016, 3, 2, 10, 40, 0), 4.963, 6.231);

        public async Task CleanTable()
        {
            int amount = await sensordataController.CountDataRecords();
            int lastIndex = amount - 1;

            for (var i = lastIndex; i >= 0; i--)
            {
                Sensordata deletedDataRecord = await sensordataController.DeleteData(i);
            }
        }

        [TestMethod]
        public async Task FindDataRecord_WithExistingId_ReturnsSensordata()
        {
            bool found;

            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata foundDataRecord = await sensordataController.FindDataRecord(0);

            if (foundDataRecord == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(dataRecord.Time, foundDataRecord.Time);
            Assert.AreEqual(dataRecord.Flow, foundDataRecord.Flow);
            Assert.AreEqual(dataRecord.Volume, foundDataRecord.Volume);
        }

        [TestMethod]
        public async Task FindDataRecord_WithNotExistingId_ReturnsNull()
        {
            bool found;

            await CleanTable();

            Sensordata foundDataRecord = await sensordataController.FindDataRecord(5);

            if (foundDataRecord == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundDataRecord);
        }

        [TestMethod]
        public async Task CountDataRecords_WithZeroDataRecordsInTable_ReturnsZero()
        {
            await CleanTable();

            int amount = await sensordataController.CountDataRecords();

            Assert.AreEqual(0, amount);
        }

        [TestMethod]
        public async Task CountDataRecords_WithOneDataRecordInTable_ReturnsOne()
        {
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            int amount = await sensordataController.CountDataRecords();

            Assert.AreEqual(1, amount);
        }

        [TestMethod]
        public async Task CountExercises_WithTwoExercisesInTable_ReturnsTwo()
        {
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata createdDataRecord2 = await sensordataController.CreateDataRecord(dataRecord2);
            int amount = await sensordataController.CountDataRecords();

            Assert.AreEqual(2, amount);
        }

        [TestMethod]
        public async Task GetLastDataRecord_WithZeroDataRecordsInTable_ReturnsNull()
        {
            bool found;

            await CleanTable();

            Sensordata foundDataRecord = await sensordataController.GetLastDataRecord();

            if (foundDataRecord == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundDataRecord);
        }

        [TestMethod]
        public async Task GetLastDataRecord_WithOneDataRecordInTable_ReturnsSensordata()
        {
            bool found;

            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata foundDataRecord = await sensordataController.GetLastDataRecord();

            if (foundDataRecord == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(dataRecord.Time, foundDataRecord.Time);
            Assert.AreEqual(dataRecord.Flow, foundDataRecord.Flow);
            Assert.AreEqual(dataRecord.Volume, foundDataRecord.Volume);
        }

        [TestMethod]
        public async Task GetLastDataRecord_WithTwoDataRecordsInTable_ReturnsSecondDataRecordInTable()
        {
            bool found;

            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata createdDataRecord2 = await sensordataController.CreateDataRecord(dataRecord2);
            Sensordata foundDataRecord = await sensordataController.GetLastDataRecord();

            if (foundDataRecord == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(dataRecord2.Time, foundDataRecord.Time);
            Assert.AreEqual(dataRecord2.Flow, foundDataRecord.Flow);
            Assert.AreEqual(dataRecord2.Volume, foundDataRecord.Volume);
        }

        [TestMethod]
        public async Task GetAllSensordata_WithZeroDataRecordsInTable_ReturnsEmptyList()
        {
            int amount = 0;

            await CleanTable();

            List<Sensordata> data = await sensordataController.GetAllSensordata();

            foreach (Sensordata datarecord in data)
            {
                amount++;
            }

            Assert.AreEqual(0, amount);
        }

        [TestMethod]
        public async Task GetAllSensordata_WithOneDataRecordInTable_ReturnsListWithOneDataRecord()
        {
            int amount = 0;
            List<Sensordata> dataInList = new List<Sensordata>();

            dataInList.Add(dataRecord);
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            List<Sensordata> data = await sensordataController.GetAllSensordata();

            foreach (Sensordata datarecord in data)
            {
                amount++;
            }

            Assert.AreEqual(1, amount);

            for (int i = 0; i < amount; i++)
            {
                Sensordata expectedData = dataInList[i];
                Sensordata actualData = data[i];

                Assert.AreEqual(expectedData.Time, actualData.Time);
                Assert.AreEqual(expectedData.Flow, actualData.Flow);
                Assert.AreEqual(expectedData.Volume, actualData.Volume);
            }
        }

        [TestMethod]
        public async Task GetAllSensordata_WithTwoDataRecordsInTable_ReturnsListWithTwoDataRecords()
        {
            int amount = 0;
            List<Sensordata> dataInList = new List<Sensordata>();

            dataInList.Add(dataRecord);
            dataInList.Add(dataRecord2);
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata createdDataRecord2 = await sensordataController.CreateDataRecord(dataRecord2);
            List<Sensordata> data = await sensordataController.GetAllSensordata();

            foreach (Sensordata datarecord in data)
            {
                amount++;
            }

            Assert.AreEqual(2, amount);

            for (int i = 0; i < amount; i++)
            {
                Sensordata expectedData = dataInList[i];
                Sensordata actualData = data[i];

                Assert.AreEqual(expectedData.Time, actualData.Time);
                Assert.AreEqual(expectedData.Flow, actualData.Flow);
                Assert.AreEqual(expectedData.Volume, actualData.Volume);
            }
        }

        [TestMethod]
        public async Task GetDataRecord_WithExistingId_ReturnsSensordata()
        {
            bool found;

            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata foundDataRecord = await sensordataController.GetDataRecord(0);

            if (foundDataRecord == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(dataRecord.Time, foundDataRecord.Time);
            Assert.AreEqual(dataRecord.Flow, foundDataRecord.Flow);
            Assert.AreEqual(dataRecord.Volume, foundDataRecord.Volume);
        }

        [TestMethod]
        public async Task GetDataRecord_WithNotExistingId_ReturnsNull()
        {
            bool found;

            await CleanTable();

            Sensordata foundDataRecord = await sensordataController.GetDataRecord(5);

            if (foundDataRecord == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundDataRecord);
        }

        [TestMethod]
        public async Task CreateDataRecord_ReturnsSensordata()
        {
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);

            Assert.AreEqual(dataRecord.Time, createdDataRecord.Time);
            Assert.AreEqual(dataRecord.Flow, createdDataRecord.Flow);
            Assert.AreEqual(dataRecord.Volume, createdDataRecord.Volume);
        }

        [TestMethod]
        public async Task UpdateDataRecord_WithExistingId_ReturnsSensordata()
        {
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata updatedDataRecord = await sensordataController.UpdateDataRecord(0, dataRecord3);

            Assert.AreEqual(dataRecord3.Time, updatedDataRecord.Time);
            Assert.AreEqual(dataRecord3.Flow, updatedDataRecord.Flow);
            Assert.AreEqual(dataRecord3.Volume, updatedDataRecord.Volume);
        }

        [TestMethod]
        public async Task UpdateDataRecord_WithNotExistingId_ReturnsNull()
        {
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata updatedDataRecord = await sensordataController.UpdateDataRecord(5, dataRecord3);

            Assert.AreEqual(null, updatedDataRecord);
        }

        [TestMethod]
        public async Task DeleteData_WithExistingId_ReturnsSensordata()
        {
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata deletedDataRecord = await sensordataController.DeleteData(0);

            Assert.AreEqual(dataRecord.Time, deletedDataRecord.Time);
            Assert.AreEqual(dataRecord.Flow, deletedDataRecord.Flow);
            Assert.AreEqual(dataRecord.Volume, deletedDataRecord.Volume);
        }

        [TestMethod]
        public async Task DeleteData_WithNotExistingId_ReturnsNull()
        {
            await CleanTable();

            Sensordata createdDataRecord = await sensordataController.CreateDataRecord(dataRecord);
            Sensordata deletedDataRecord = await sensordataController.DeleteData(5);

            Assert.AreEqual(null, deletedDataRecord);
        }
    }
}
