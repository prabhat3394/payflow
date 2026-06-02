using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PayFlow.Identity.Api.Domain.Entities;

namespace PayFlow.Identity.Api.Application.Commands
{
    public record RegisterUserCommand(string FullName, string Email, string Password) : IRequest<RegisterUserResult>;
    
    public record RegisterUserResult(bool Succeeded,string? UserId, List<string> Errors);

    public class  RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full name is required.")
                 .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");                               

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")   ;
        }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
    {
        private readonly UserManager<AppUser> _userManager;
        public RegisterUserCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new AppUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return new RegisterUserResult(false, null, errors);
            }
            
            return new RegisterUserResult(true, user.Id, []);
        }
    }
}
