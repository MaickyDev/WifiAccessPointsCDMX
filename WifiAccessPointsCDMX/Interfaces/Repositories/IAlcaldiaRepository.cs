using WifiAccessPointsCDMX.Models;

namespace WifiAccessPointsCDMX.Interfaces.Repositories
{
    public interface IAlcaldiaRepository
    {
        Task<List<AlcaldiaModel>> GetAllAsync();
        Task BulkInsertAsync(List<AlcaldiaModel> items);
    }
}