namespace BlackJack.Sessions.Core.Abstractions.DomainModels;

public interface ISession
{
    Guid OwnerId { get; }
    string Name { get; }
    string Code { get; }

    bool IsOwner(Guid userId);
    void SetName(string value);
    void SetCode(string value);
}