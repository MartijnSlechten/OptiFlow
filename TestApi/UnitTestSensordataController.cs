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

        public async Task CleanTable()
        {
            int amount = await sensordataController.CountDataRecords();
            int lastIndex = amount - 1;

            for (var i = lastIndex; i >= 0; i--)
            {
                Sensordata deletedDataRecord = await sensordataController.DeleteData(i);
            }
        }
    }
}
