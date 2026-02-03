using WifiAccessPointsCDMX.Interfaces.Repositories;

namespace WifiAccessPointsCDMX.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProgramRepository Programs { get; }
        IAlcaldiaRepository Alcaldias { get; }
        IAccessPointRepository AccessPoints { get; }

        Task<int> SaveChangesAsync();
    }
}