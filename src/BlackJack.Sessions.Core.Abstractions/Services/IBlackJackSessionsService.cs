using BlackJack.Sessions.Core.Abstractions.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;

namespace BlackJack.Sessions.Core.Abstractions.Services;

public interface IBlackJackSessionsService
{
    Task<SessionDetailsDto> GetSessionByCodeAsync(Guid userId, string code, CancellationToken ct = default);
    Task<SessionDetailsDto> GetSessionByIdAsync(Guid userId, Guid id, CancellationToken ct = default);
    Task<SessionDetailsDto> CreateSessionAsync(SessionCreateDto dto, CancellationToken ct = default);
    Task<SessionDetailsDto> UpdateSessionAsync(Guid userId, Guid id, SessionDetailsDto dto, CancellationToken ct = default);
    Task<SessionDetailsDto> PatchSessionAsync(Guid userId, Guid id, JsonPatchDocument<SessionDetailsDto> dto, CancellationToken ct);
}