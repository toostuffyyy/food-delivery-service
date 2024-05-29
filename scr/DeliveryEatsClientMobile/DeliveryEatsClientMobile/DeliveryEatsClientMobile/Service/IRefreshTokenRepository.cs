using System.Threading.Tasks;

namespace DeliveryEatsClientMobile.Service
{
    public interface IRefreshTokenRepository
    {
        public Task<string?> GetRefreshToken();
        public Task AddRefreshToken(string token);
        public Task UpdateRefreshToken(string token);
        public Task DeleteRefreshToken();
    }
}
