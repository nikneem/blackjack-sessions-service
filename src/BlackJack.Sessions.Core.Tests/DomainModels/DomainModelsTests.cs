using BlackJack.Sessions.Core.DomainModels;
using FluentAssertions;

namespace BlackJack.Sessions.Core.Tests.DomainModels;

public class DomainModelsTests
{
    [Fact]
    public void WhenSessionIsCreated_ItHasASessionCode()
    {
        var userId = Guid.NewGuid();
        var session = Session.Create(userId,"Test");
        session.Code.Should().NotBeNullOrEmpty();
        session.Code.Should().HaveLength(6);
    }

    [Fact]
    public void WhenSessionIsCreated_TheNamePropertyIsSet()
    {
        var userId = Guid.NewGuid();
        var sessionName = "Session Name";
        var session = Session.Create(userId,sessionName);
        session.Name.Should().Be(sessionName);
    }

    [Fact]
    public void WhenSessionCodeIsChanged_TheCodePropertyIsSet()
    {
        var sessionName = "Session Name";
        var targetCode = "newcode";
        var userId = Guid.NewGuid();
        var session = Session.Create(userId,sessionName);
        session.SetCode(targetCode);
        session.Code.Should().Be(targetCode);
    }
}