using BlackJack.Sessions.Core.Abstractions.DataTransferObjects;
using BlackJack.Sessions.Core.Abstractions.Exceptions;
using BlackJack.Sessions.Core.Abstractions.Repositories;
using BlackJack.Sessions.Core.Abstractions.Services;
using BlackJack.Sessions.Core.DomainModels;

namespace BlackJack.Sessions.Core.Services;

public class BlackJackSessionsService: IBlackJackSessionsService

{
    private readonly IBlackJackSessionsRepository _repository;

    public Task<SessionDetailsDto> GetSessionByCodeAsync(string code, CancellationToken ct = default)
    {
        return _repository.GetSessionByCodeAsync(code, ct);
    }

    public Task<SessionDetailsDto> GetSessionByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _repository.GetSessionByIdAsync(id, ct);
    }

    public async Task<SessionDetailsDto> CreateSessionAsync(SessionCreateDto dto, CancellationToken ct = default)
    {
        var session = Session.Create(dto.Name);
        if (await _repository.PersistAsync(session, ct))
        {
            return new SessionDetailsDto
            {
                Id = session.Id,
                Name = session.Name,
                Code = session.Code
            };
        }
        throw new BlackJackSessionCreateException();
    }

    public async Task<SessionDetailsDto> UpdateSessionAsync(Guid id, SessionDetailsDto dto, CancellationToken ct = default)
    {
        var session = await _repository.GetAsync(id, ct);
        session.SetCode(dto.Code);
        session.SetName(dto.Name);
        if (await _repository.PersistAsync(session, ct))
        {
            return new SessionDetailsDto
            {
                Id = id,
                Name = session.Name,
                Code = session.Code
            };
        }
        throw new BlackJackSessionCreateException();
    }

    public BlackJackSessionsService(IBlackJackSessionsRepository repository)
    {
        _repository = repository;
    }
}