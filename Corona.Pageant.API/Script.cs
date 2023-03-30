using System;
using Azure;
using Azure.Data.Tables;

namespace Corona.Pageant.API;

public class Script : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Text { get; set; }
    public string SwitchToScene { get; set; }
    public string Camera1Action { get; set; }
    public string Camera1Position { get; set; }
    public string Camera2Action { get; set; }
    public string Camera2Position { get; set; }
    public string Camera3Position { get; set; }
    public string Camera3Action { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}