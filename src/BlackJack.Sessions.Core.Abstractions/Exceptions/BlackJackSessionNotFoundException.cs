using BlackJack.Core.Exceptions;
using BlackJack.Sessions.Core.Abstractions.ErrorCodes;

namespace BlackJack.Sessions.Core.Abstractions.Exceptions;

public class BlackJackSessionNotFoundException : BlackJackException
{
    public BlackJackSessionNotFoundException(string sessionCode, Exception? ex = null) 
        : base(BlackJackSessionsErrorCode.NotFound, $"A session with session code {sessionCode} was not found", ex)
    {
    }
}