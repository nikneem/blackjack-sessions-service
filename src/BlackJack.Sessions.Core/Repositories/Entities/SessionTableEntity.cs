using Azure;
using Azure.Data.Tables;

namespace BlackJack.Sessions.Core.Repositories.Entities;

public class SessionTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = null!;
    public string RowKey { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; } = ETag.All;
}