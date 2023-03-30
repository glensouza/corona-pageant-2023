using System;
using Azure;
using Azure.Data.Tables;

namespace Corona.Pageant.API
{
    public class Settings : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string Setting { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
