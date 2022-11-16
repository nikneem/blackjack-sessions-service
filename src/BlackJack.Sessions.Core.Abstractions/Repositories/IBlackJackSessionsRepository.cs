using BlackJack.Sessions.Core.Abstractions.DataTransferObjects;
using BlackJack.Sessions.Core.Abstractions.DomainModels;

namespace BlackJack.Sessions.Core.Abstractions.Repositories;

public interface IBlackJackSessionsRepository
{
    Task<SessionDetailsDto> GetSessionByCodeAsync(string code, CancellationToken ct = default);
    Task<SessionDetailsDto> GetSessionByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> GetIsSessionCodeUnique(Guid id, string code, CancellationToken ct = default);
    Task<ISession> GetAsync(Guid id, CancellationToken ct = default);
    Task<bool> PersistAsync(ISession domainModel, CancellationToken ct = default);
}