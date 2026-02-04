using WifiAccessPointsCDMX.Models;

namespace WifiAccessPointsCDMX.Interfaces.Repositories
{
    public interface IAccessPointRepository
    {
        Task<List<string>> GetAllCodesAsync();
        Task BulkInsertAsync(List<AccessPointModel> items);
        Task<int> CountAsync();
        Task<int> CountByAlcaldiaAsync(int alcaldiaId);
        Task<List<AccessPointModel>> GetPagedAsync(int skip, int take);
        Task<List<AccessPointModel>> GetPagedByAlcaldiaAsync(int alcaldiaId, int skip, int take);
        Task<AccessPointModel?> GetByExternalIdAsync(string externalId);
        Task<List<AccessPointModel>> GetNearbyPagedAsync(double lat, double lng, int skip, int take);

        // GraphQL
        IQueryable<AccessPointModel> GetQueryable();
    }
}