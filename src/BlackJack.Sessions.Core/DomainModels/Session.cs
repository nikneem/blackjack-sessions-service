using BlackJack.Sessions.Core.Abstractions.DomainModels;
using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;

namespace BlackJack.Sessions.Core.DomainModels;

public class Session: DomainModel<Guid>, ISession
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; }

    public void SetName(string value)
    {
        if (!Equals(Name, value))
        {
            Name = value;
            SetState(TrackingState.Modified);
        }
    }
    public void SetCode(string value)
    {
        if (!Equals(Code, value))
        {
            Code = value;
            SetState(TrackingState.Modified);
        }
    }

    private static string GenerateCode(int length = 6)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var stringChars = new char[length];
        var random = new Random();

        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }

    internal Session(Guid id, string name, string code) : base(id)
    {
        Name = name;
        Code = code;
    }
    private Session() : base(Guid.NewGuid(), TrackingState.New)
    {
        Code = GenerateCode();
    }
    public static Session Create(string name)
    {
        var session = new Session();
        session.SetName(name);
        return session;
    }
}