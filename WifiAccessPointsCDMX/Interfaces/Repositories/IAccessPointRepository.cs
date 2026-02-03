using WifiAccessPointsCDMX.Models;

namespace WifiAccessPointsCDMX.Interfaces.Repositories
{
    public interface IAccessPointRepository
    {
        Task<List<string>> GetAllCodesAsync();
        Task BulkInsertAsync(List<AccessPointModel> items);
    }
}