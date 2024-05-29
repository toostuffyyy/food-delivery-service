using desktop.Models;
using Refit;
using System.Threading.Tasks;

namespace desktop.Service
{
    public interface IAuthorizationRepository
    {
        [Post("/authorization/loginemployee")]
        public Task<Token> LoginEmployee([Body] Authorization authorization);
        [Post("/authorization/updatetokenemployee")]
        public Task<Token> UpdateTokenEmployee([Header("RefreshToken")] string refreshToken);
        [Post("/authorization/logoutemployee")]
        public Task LogoutEmployee([Authorize("Bearer")] string accessToken);
    }
}
