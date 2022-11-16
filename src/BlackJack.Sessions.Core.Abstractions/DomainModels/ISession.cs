namespace BlackJack.Sessions.Core.Abstractions.DomainModels;

public interface ISession
{
    string Name { get; }
    string Code { get; }

    void SetName(string value);
    void SetCode(string value);
}