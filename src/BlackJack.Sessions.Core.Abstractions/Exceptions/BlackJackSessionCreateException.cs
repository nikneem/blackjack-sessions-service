using BlackJack.Sessions.Core.Abstractions.ErrorCodes;

namespace BlackJack.Sessions.Core.Abstractions.Exceptions;

public class BlackJackSessionCreateException : BlackJackSessionsException
{
    public BlackJackSessionCreateException(Exception? ex = null) 
        : base(BlackJackSessionsErrorCode.CreateFailure, "Failed to create or update session in persistence", ex)
    {
    }
}