using System.Linq;
using System.Threading.Tasks;
using DeliveryEatsClientMobile.Context;
using Microsoft.EntityFrameworkCore;

namespace DeliveryEatsClientMobile.Service
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SqLiteContext _context;
        public RefreshTokenRepository(SqLiteContext sqLiteContext)
        {
            _context = sqLiteContext;
        }
        
        public async Task<string?> GetRefreshToken()
        {
            var refreshToken = await _context.refreshTokens.FirstOrDefaultAsync();
            return refreshToken != null ? refreshToken.Token : null;
        }
        
        public async Task AddRefreshToken(string token)
        {
            _context.refreshTokens.Add(new() { Token = token });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRefreshToken()
        {
            _context.refreshTokens.Remove(_context.refreshTokens.FirstOrDefault());
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshToken(string token)
        {
            DeleteRefreshToken();
            AddRefreshToken(token);
        }
    }
}