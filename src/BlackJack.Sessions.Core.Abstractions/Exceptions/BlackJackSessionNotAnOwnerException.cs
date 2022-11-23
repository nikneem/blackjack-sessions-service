using BlackJack.Sessions.Core.Abstractions.ErrorCodes;

namespace BlackJack.Sessions.Core.Abstractions.Exceptions;

public class BlackJackSessionNotAnOwnerException : BlackJackSessionsException
{
    public BlackJackSessionNotAnOwnerException(Guid userId, Exception? ex = null) : base(
        BlackJackSessionsErrorCode.NotAnOwner, 
        $"User {userId} is not the owner of this session and cannot modify it", 
        ex)
    {
    }
}