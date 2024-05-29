using desktop.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Service
{
    public class RefreshTokenWindowsRepository : IRefreshTokenRepository
    {
        private readonly byte[] entropy = { 1, 5, 6, 12, 5, 62, 56, 1, 56, 3 };
        private readonly SqLiteContext _context;
        public RefreshTokenWindowsRepository(SqLiteContext sqLiteContext)
        {
            _context = sqLiteContext;
        }

        public async Task AddRefreshToken(string token)
        {
            string protictToken = await Task.Run(() => ProtectToken(token));
            _context.refreshTokens.Add(new() { Token=protictToken});
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRefreshToken()
        {
            _context.refreshTokens.Remove(_context.refreshTokens.FirstOrDefault());
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetRefreshToken()
        {
            var refreshToken = await _context.refreshTokens.FirstOrDefaultAsync();
            if (refreshToken == null)
                return null;
            string token = await Task.Run(() => UnProtectToken(refreshToken.Token));
            return token;
        }

        public async Task UpdateRefreshToken(string token)
        {
            DeleteRefreshToken();
            AddRefreshToken(token);
        }

        private string ProtectToken(string token)
        {
            byte[] tokenBytes = Encoding.Unicode.GetBytes(token);
            byte[] encryptBytes = ProtectedData.Protect(tokenBytes, entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptBytes);
        }
        private string UnProtectToken(string protectToken)
        {
            byte[] encryptBytes = Convert.FromBase64String(protectToken);
            byte[] tokenBytes = ProtectedData.Unprotect(encryptBytes, entropy, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(tokenBytes);
        }
    }
}
