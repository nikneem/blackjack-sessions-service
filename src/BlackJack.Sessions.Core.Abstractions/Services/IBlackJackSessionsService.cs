using BlackJack.Sessions.Core.Abstractions.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;

namespace BlackJack.Sessions.Core.Abstractions.Services;

public interface IBlackJackSessionsService
{
    Task<SessionDetailsDto> GetSessionByCodeAsync(string code, CancellationToken ct = default);
    Task<SessionDetailsDto> GetSessionByIdAsync(Guid id, CancellationToken ct = default);
    Task<SessionDetailsDto> CreateSessionAsync(SessionCreateDto dto, CancellationToken ct = default);
    Task<SessionDetailsDto> UpdateSessionAsync(Guid id, SessionDetailsDto dto, CancellationToken ct = default);
    Task<SessionDetailsDto> PatchSessionAsync(Guid id, JsonPatchDocument<SessionDetailsDto> dto, CancellationToken ct);
}