using BlackJack.Core.Exceptions;
using BlackJack.Sessions.Core.Abstractions.ErrorCodes;

namespace BlackJack.Sessions.Core.Abstractions.Exceptions;

public class BlackJackSessionsException : BlackJackException
{
    protected BlackJackSessionsException(BlackJackSessionsErrorCode errorCode, string message, Exception? ex) : base(errorCode, message, ex)
    {
    }
}