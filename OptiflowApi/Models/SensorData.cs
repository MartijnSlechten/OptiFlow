﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptiflowApi.Models
{
    public class Sensordata : TableEntity
    {
        public Sensordata(long id)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = id.ToString();
            this.Id = id;
        }

        public Sensordata(long id, DateTime time, double flow, double volume)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = id.ToString();
            this.Id = id;
            this.Time = time;
            this.Flow = Flow;
            this.Volume = Volume;
        }

        public Sensordata() { }

        public long Id { get; set; }

        public DateTime Time { get; set; }

        public double Flow { get; set; }

        public double Volume { get; set; }
    }
}