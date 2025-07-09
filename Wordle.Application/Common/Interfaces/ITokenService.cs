using Wordle.Application.Users.DTOs;
using Wordle.Domain.Users;

namespace Wordle.Application.Common.Interfaces
{
    public interface ITokenService
    {      
        TokenResultDto CreateToken(User user);
    }

}
