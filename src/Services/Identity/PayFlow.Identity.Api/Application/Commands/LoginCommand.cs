using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PayFlow.Identity.Api.Domain.Entities;
using PayFlow.Identity.Api.Infrastructure.Services;

namespace PayFlow.Identity.Api.Application.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;
    public record LoginResult(bool Succeeded, string? Errors, string? AccessToken, string? RefreshToken);
    public class LoginCommandValidator : FluentValidation.AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        public LoginCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
            {
                return new LoginResult(false, "Invalid credentials.", null, null);
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
            {
                return new LoginResult(false, "Invalid credentials.", null, null);
            }
            
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            return new LoginResult(true, null, accessToken, refreshToken);

        }
    }
}
