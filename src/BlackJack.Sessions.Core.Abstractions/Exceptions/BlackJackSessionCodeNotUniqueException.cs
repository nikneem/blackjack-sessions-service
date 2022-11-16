using BlackJack.Sessions.Core.Abstractions.ErrorCodes;

namespace BlackJack.Sessions.Core.Abstractions.Exceptions;

public class BlackJackSessionCodeNotUniqueException : BlackJackSessionsException
{
    public BlackJackSessionCodeNotUniqueException(string code, Exception? ex = null) 
        : base( BlackJackSessionsErrorCode.CodeNotUnique, $"The session code {code} is not unique", ex)
    {
    }
}