using WifiAccessPointsCDMX.Models;

namespace WifiAccessPointsCDMX.Interfaces.Repositories
{
    public interface IAlcaldiaRepository
    {
        Task<List<AlcaldiaModel>> GetAllAsync();
        Task<AlcaldiaModel?> GetByNameAsync(string name);
        Task AddAsync(AlcaldiaModel alcaldia);
        Task BulkInsertAsync(List<AlcaldiaModel> items);

    }
}