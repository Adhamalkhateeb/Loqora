using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;
using MediatR;

namespace Loqora.Application.Features.Identity.Commands.RefreshTokens;

public sealed record RefreshTokenCommand(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<TokenResponse>>;
