using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;

public record GenerateTokenCommand(
    string Email,
    string Password)
    : ICommand<TokenResponse>;