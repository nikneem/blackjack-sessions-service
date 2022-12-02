using System.Security.Cryptography;
using Azure;
using Azure.Data.Tables;
using BlackJack.Core.Factories;
using BlackJack.Sessions.Core.Abstractions.DataTransferObjects;
using BlackJack.Sessions.Core.Abstractions.DomainModels;
using BlackJack.Sessions.Core.Abstractions.Exceptions;
using BlackJack.Sessions.Core.Abstractions.Repositories;
using BlackJack.Sessions.Core.DomainModels;
using BlackJack.Sessions.Core.Repositories.Entities;
using HexMaster.DomainDrivenDesign.ChangeTracking;

namespace BlackJack.Sessions.Core.Repositories;

public class BlackJackSessionsRepository: IBlackJackSessionsRepository
{
    private readonly IStorageTableClientFactory _tableStorageClientFactory;

    private const string TableName = "sessions";
    private const string PartitionKey = "session";

    public async Task<SessionDetailsDto> GetSessionByCodeAsync(Guid userId, string code, CancellationToken ct = default)
    {
        var tableClient = _tableStorageClientFactory.CreateClient(TableName);
        var pollsQuery = tableClient.QueryAsync<SessionTableEntity>($"{nameof(SessionTableEntity.PartitionKey)} eq '{PartitionKey}' and {nameof(SessionTableEntity.Code)} eq '{code}'", cancellationToken: ct);

        var sessionsList = new List<SessionDetailsDto>();
        await foreach (var page in pollsQuery.AsPages())
        {
            sessionsList.AddRange(page.Values.Select(entity =>
            new SessionDetailsDto
            {
                Id = Guid.Parse(entity.RowKey),
                Code = entity.Code,
                Name = entity.Name,
                IsOwner = Equals(entity.OwnerId, userId),

            }));
        }
        if (sessionsList.Count == 1)
        {
            return sessionsList.First();
        }

        throw new BlackJackSessionNotFoundException(code);
    }

    public async Task<SessionDetailsDto> GetSessionByIdAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var tableClient = _tableStorageClientFactory.CreateClient(TableName);
        var entity = await tableClient.GetEntityAsync<SessionTableEntity>(PartitionKey, id.ToString(), null, ct);
        return new SessionDetailsDto
        {
            Id = Guid.Parse(entity.Value.RowKey),
            Code = entity.Value.Code,
            Name = entity.Value.Name,
            IsOwner = Equals(entity.Value.OwnerId, userId),
        };
    }

    public async  Task<bool> GetIsSessionCodeUnique(Guid id, string code, CancellationToken ct = default)
    {
        var tableClient = _tableStorageClientFactory.CreateClient(TableName);
        var pollsQuery = tableClient.QueryAsync<SessionTableEntity>($"{nameof(SessionTableEntity.PartitionKey)} eq '{PartitionKey}' and {nameof(SessionTableEntity.RowKey)} ne '{id}' and {nameof(SessionTableEntity.Code)} eq '{code}'", cancellationToken: ct);

        var sessionsEnumeration = pollsQuery.GetAsyncEnumerator(ct);
        return !await sessionsEnumeration.MoveNextAsync();
    }

    public async Task<ISession> GetAsync( Guid id, CancellationToken ct = default)
    {
        var tableClient = _tableStorageClientFactory.CreateClient(TableName);
        var entity = await tableClient.GetEntityAsync<SessionTableEntity>(PartitionKey, id.ToString(), null, ct);
        return new Session(
            Guid.Parse(entity.Value.RowKey),
            entity.Value.OwnerId,
            entity.Value.Name,
            entity.Value.Code);
    }

    public async Task<bool> PersistAsync(ISession domainModel, CancellationToken ct = default)
    {
        var tableClient = _tableStorageClientFactory.CreateClient(TableName);
        if (domainModel is Session session)
        {
            var entity = new SessionTableEntity
            {
                PartitionKey = PartitionKey,
                RowKey = session.Id.ToString(),
                Name = session.Name,
                Code = session.Code,
                ETag = ETag.All,
                Timestamp = DateTimeOffset.UtcNow
            };
            if (session.TrackingState == TrackingState.New)
            {
                var azureCreateResponse = await tableClient.AddEntityAsync(entity, ct);
                return !azureCreateResponse.IsError;
            }

            if (session.TrackingState == TrackingState.Modified)
            {
                var azureUpdateResponse =
                    await tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace, ct);
                return !azureUpdateResponse.IsError;
            }
        }

        throw new BlackJackSessionCreateException();
    }

    public BlackJackSessionsRepository(IStorageTableClientFactory tableStorageClientFactory)
    {
        _tableStorageClientFactory = tableStorageClientFactory;
    }

}