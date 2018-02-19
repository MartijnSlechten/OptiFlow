using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptiflowApi.Models
{
    public class Exercise : TableEntity
    {
        public Exercise(long id)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = id.ToString();
            this.Id = id;
        }

        public Exercise(long id, string name, string description, string imageSource)
        {
            this.PartitionKey = id.ToString();
            this.RowKey = id.ToString();
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.ImageSource = imageSource;
        }

        public Exercise() { }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageSource { get; set; }
    }
}
