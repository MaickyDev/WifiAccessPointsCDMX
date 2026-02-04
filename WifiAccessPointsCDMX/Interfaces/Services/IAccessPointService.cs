using WifiAccessPointsCDMX.Models;
using WifiAccessPointsCDMX.Models.Dto;

namespace WifiAccessPointsCDMX.Interfaces.Services
{
    public interface IAccessPointService
    {
        Task<PagedResult<AccessPointModel>> GetPagedAsync(int page, int pageSize);
        Task<PagedResult<AccessPointModel>> GetByExternalIdAsync(string externalId);
        Task<PagedResult<AccessPointModel>> GetByAlcaldiaAsync(int alcaldiaId, int page, int pageSize);
        Task<PagedResult<AccessPointModel>> GetNearbyAsync(double lat, double lng, int page, int pageSize);
    }
}
