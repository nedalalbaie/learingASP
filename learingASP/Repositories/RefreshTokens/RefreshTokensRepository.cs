using learingASP.Data;
using learingASP.Models;
using Microsoft.EntityFrameworkCore;

namespace learingASP.Repositories.RefreshTokens
{
    public class RefreshTokensRepository(AppDbContext context) : IRefreshTokensRepository
    {
        public void AddRefreshToken(RefreshToken refreshToken)
        {
            context.RefreshTokens.Add(refreshToken);
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await context.RefreshTokens
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }
    }
}
