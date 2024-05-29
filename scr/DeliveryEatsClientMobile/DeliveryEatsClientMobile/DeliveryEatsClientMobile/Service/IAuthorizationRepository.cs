using Refit;
using System.Threading.Tasks;
using DeliveryEatsClientMobile.Models;

namespace DeliveryEatsClientMobile.Service
{
    public interface IAuthorizationRepository
    {
        [Get("/authorization/checkclient")]
        public Task<bool> CheckClient(string login);
        [Post("/authorization/loginclient")]
        public Task<Token> LoginClient([Body] Authorization authorization);
        [Post("/authorization/updatetokenclient")]
        public Task<Token> UpdateTokenClient([Header("RefreshToken")] string refreshToken);
        [Post("/authorization/logoutclient")]
        public Task LogoutClient([Authorize("Bearer")] string accessToken);
    }
}
