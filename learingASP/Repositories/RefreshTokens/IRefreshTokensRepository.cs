using learingASP.Models;

namespace learingASP.Repositories.RefreshTokens
{
    public interface IRefreshTokensRepository
    {
        void AddRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    }
}
