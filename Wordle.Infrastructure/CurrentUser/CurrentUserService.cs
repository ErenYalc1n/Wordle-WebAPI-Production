using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wordle.Application.Common.Interfaces;

namespace Wordle.Infrastructure.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(userId, out var parsedId) ? parsedId : Guid.Empty;
            }
        }
    }
}
