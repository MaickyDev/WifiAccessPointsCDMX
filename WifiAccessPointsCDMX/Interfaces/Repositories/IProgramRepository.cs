using WifiAccessPointsCDMX.Models;

namespace WifiAccessPointsCDMX.Interfaces.Repositories
{
    public interface IProgramRepository
    {
        Task<List<ProgramModel>> GetAllAsync();
        Task BulkInsertAsync(List<ProgramModel> items);
    }
}