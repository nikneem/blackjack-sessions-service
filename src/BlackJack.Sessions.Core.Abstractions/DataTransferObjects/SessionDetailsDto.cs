namespace BlackJack.Sessions.Core.Abstractions.DataTransferObjects;

public class SessionDetailsDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsOwner { get; set; }
}