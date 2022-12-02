using BlackJack.Core.ErrorCodes;

namespace BlackJack.Sessions.Core.Abstractions.ErrorCodes;

public abstract class BlackJackSessionsErrorCode : BlackJackErrorCode
{
    public static readonly BlackJackSessionsErrorCode NotFound = new BlackJackSessionNotFoundErrorCode();
    public static readonly BlackJackSessionsErrorCode CreateFailure = new BlackJackSessionCreateFailureErrorCode();
    public static readonly BlackJackSessionsErrorCode CodeNotUnique = new BlackJackSessionCodeNotUniqueErrorCode();
    public static readonly BlackJackSessionsErrorCode NotAnOwner = new BlackJackSessionNotAnOwnerErrorCode();

    public override string ErrorNamespace => "Errors.Sessions";
}
