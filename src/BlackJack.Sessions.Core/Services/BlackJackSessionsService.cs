﻿using Azure;
using BlackJack.Sessions.Core.Abstractions.DataTransferObjects;
using BlackJack.Sessions.Core.Abstractions.Exceptions;
using BlackJack.Sessions.Core.Abstractions.Repositories;
using BlackJack.Sessions.Core.Abstractions.Services;
using BlackJack.Sessions.Core.DomainModels;
using Microsoft.AspNetCore.JsonPatch;
using System.Globalization;

namespace BlackJack.Sessions.Core.Services;

public class BlackJackSessionsService: IBlackJackSessionsService

{
    private readonly IBlackJackSessionsRepository _repository;

    public Task<SessionDetailsDto> GetSessionByCodeAsync(Guid userId, string code, CancellationToken ct = default)
    {
        return _repository.GetSessionByCodeAsync(userId, code, ct);
    }

    public Task<SessionDetailsDto> GetSessionByIdAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        return _repository.GetSessionByIdAsync(userId, id, ct);
    }

    public async Task<SessionDetailsDto> CreateSessionAsync(SessionCreateDto dto, CancellationToken ct = default)
    {
        try
        {
            var session = Session.Create(dto.UserId, dto.Name);
            if (await _repository.PersistAsync(session, ct))
            {
                return new SessionDetailsDto
                {
                    Id = session.Id,
                    Name = session.Name,
                    Code = session.Code,
                    IsOwner = session.IsOwner(dto.UserId)
                };
            }
        }
        catch (Exception ex)
        {
            throw new BlackJackSessionCreateException(ex);
        }

        throw new BlackJackSessionCreateException();
    }

    public async Task<SessionDetailsDto> UpdateSessionAsync(Guid userId, Guid id, SessionDetailsDto dto, CancellationToken ct = default)
    {
        var session = await _repository.GetAsync(id, ct);
        if (!session.IsOwner(userId))
        {
            throw new BlackJackSessionNotAnOwnerException(userId);
        }
        if (!string.IsNullOrWhiteSpace(dto.Code) && !Equals(session.Code, dto.Code))
        {
            if (await _repository.GetIsSessionCodeUnique(id, dto.Code, ct))
            {
                session.SetCode(dto.Code);
            }
            else
            {
                throw new BlackJackSessionCodeNotUniqueException(dto.Code);
            }
        }
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

    public async Task<SessionDetailsDto> PatchSessionAsync(Guid userId, Guid id, JsonPatchDocument<SessionDetailsDto> dto, CancellationToken ct)
    {
        var session = await _repository.GetAsync(id, ct);
        if (!session.IsOwner(userId))
        {
            throw new BlackJackSessionNotAnOwnerException(userId);
        }
        var serverSideDto= new SessionDetailsDto
        {
            Id = id,
            Name = session.Name,
            Code = session.Code
        };
        dto.ApplyTo(serverSideDto);


        if (!string.IsNullOrWhiteSpace(serverSideDto.Code) && !Equals(session.Code, serverSideDto.Code))
        {
            if (await _repository.GetIsSessionCodeUnique(id, serverSideDto.Code, ct))
            {
                session.SetCode(serverSideDto.Code);
            }
            else
            {
                throw new BlackJackSessionCodeNotUniqueException(serverSideDto.Code);
            }
        }
        session.SetName(serverSideDto.Name);
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