using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Tokens.RefreshToken;

public record RefreshTokenCommand(string Token, string RefreshToken)
    : ICommand<RefreshTokenCommandResponse>;