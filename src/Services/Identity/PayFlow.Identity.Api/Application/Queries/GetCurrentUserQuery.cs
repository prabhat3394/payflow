using MediatR;
using Microsoft.AspNetCore.Identity;
using PayFlow.Identity.Api.Domain.Entities;
using System.Security.Claims;

namespace PayFlow.Identity.Api.Application.Queries
{
    public record GetCurrentUserQuery(ClaimsPrincipal User) : IRequest<CurrentUserResult>;
    public record CurrentUserResult(
        bool Found,
        string? UserId,
        string? FullName,
        string? Email,
        DateTime? CreatedAt,
        string? Error);

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserResult>
    {
        private readonly UserManager<AppUser> _userManager;

        public GetCurrentUserQueryHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CurrentUserResult> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            //Read the user ID from JWT claim
            var userId = request.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrEmpty(userId))
            {
                return new CurrentUserResult(false, null, null, null, null, "User not found");
            }

            //Fetch fresh data from DB 
            var user = await _userManager.FindByIdAsync(userId);

            if(user is null || !user.IsActive)
            {
                return new CurrentUserResult(false, null, null, null, null, "User not found");
            }

            //return latest user data
            return new CurrentUserResult(
                true,
                user.Id,
                user.FullName,
                user.Email,
                user.CreatedAt,
                null);
        }
    }
}
