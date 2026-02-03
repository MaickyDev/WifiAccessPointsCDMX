using WifiAccessPointsCDMX.Models;

namespace WifiAccessPointsCDMX.Interfaces.Repositories
{
    public interface IProgramRepository
    {
        Task<List<ProgramModel>> GetAllAsync();
        Task<ProgramModel?> GetByNameAsync(string name);
        Task AddAsync(ProgramModel program);
        Task BulkInsertAsync(List<ProgramModel> items);
    }
}