using BlackJack.Sessions.Core.Abstractions.DataTransferObjects;
using BlackJack.Sessions.Core.Abstractions.Exceptions;
using BlackJack.Sessions.Core.Abstractions.Repositories;
using BlackJack.Sessions.Core.DomainModels;
using BlackJack.Sessions.Core.Services;
using FluentAssertions;
using Moq;

namespace BlackJack.Sessions.Core.Tests.Services;

public class SessionsServiceTests
{

    private readonly Mock<IBlackJackSessionsRepository> _repositoryMock;


    [Fact]
    public async Task WhenSessionCreated_TheSessionIsPersisted()
    {
        WithPersistenceSucceeding();
        var service = new BlackJackSessionsService(_repositoryMock.Object);
        await service.CreateSessionAsync(new SessionCreateDto { Name = "Test" });
        _repositoryMock.Verify(x => x.PersistAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task WhenSessionPersistenceFails_TheSessionIsPersisted()
    {
        var service = new BlackJackSessionsService(_repositoryMock.Object);
        var act = async() =>await service.CreateSessionAsync(new SessionCreateDto { Name = "Test" });
        await act.Should().ThrowAsync<BlackJackSessionCreateException>();
    }
    [Fact]
    public async Task WhenSessionNameUpdated_TheSessionIsPersisted()
    {
        var newName = "New name";
        var session = WithSessionInRepository();
        WithPersistenceSucceeding();
        var service = new BlackJackSessionsService(_repositoryMock.Object);
        var result = await service.UpdateSessionAsync(session.Id, new SessionDetailsDto { Name = newName });
        _repositoryMock.Verify(x => x.PersistAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once);
        result.Name.Should().Be(newName);
    }
    [Fact]
    public async Task WhenSessionUpdated_TheSessionIsPersisted()
    {
        var newName = "New name";
        var newCode = "special";
        var session = WithSessionInRepository();
        WithUniqueSessionCode();
        WithPersistenceSucceeding();
        var service = new BlackJackSessionsService(_repositoryMock.Object);
        var result = await service.UpdateSessionAsync(session.Id, new SessionDetailsDto { Name = newName, Code = newCode });
        _repositoryMock.Verify(x => x.PersistAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once);
        result.Name.Should().Be(newName);
        result.Code.Should().Be(newCode);
    }
    [Fact]
    public void WhenSessionCodeNotUnique_ItThrows()
    {
        var newCode = "special";
        var session = WithSessionInRepository();
        WithPersistenceSucceeding();
        var service = new BlackJackSessionsService(_repositoryMock.Object);
        var act = () => Task.FromResult(service.UpdateSessionAsync(session.Id, new SessionDetailsDto {Code = newCode}));
        act.Should().ThrowAsync<BlackJackSessionCodeNotUniqueException>();
    }


    private void WithPersistenceSucceeding()
    {
        _repositoryMock.Setup(x => x.PersistAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }
    private void WithUniqueSessionCode()
    {
        _repositoryMock.Setup(x => x.GetIsSessionCodeUnique(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }
    private Session WithSessionInRepository()
    {
        var session = new Session(Guid.NewGuid(), "Test", "Test");
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        return session;
    }

    public SessionsServiceTests()
    {
        _repositoryMock = new Mock<IBlackJackSessionsRepository>();
    }
}