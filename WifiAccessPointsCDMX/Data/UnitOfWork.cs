using WifiAccessPointsCDMX.Interfaces;
using WifiAccessPointsCDMX.Repositories;
using WifiAccessPointsCDMX.Interfaces.Repositories;

namespace WifiAccessPointsCDMX.Data
{
    // UnitOfWork coordinates all repository operations and ensures they share the same DbContext
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AccessPointsDbContext _db;

        public UnitOfWork(AccessPointsDbContext db)
        {
            _db = db;
            Programs = new ProgramRepository(db);
            Alcaldias = new AlcaldiaRepository(db);
            AccessPoints = new AccessPointRepository(db);
        }

        public IProgramRepository Programs { get; }
        public IAlcaldiaRepository Alcaldias { get; }
        public IAccessPointRepository AccessPoints { get; }

        // Commits all pending changes in the DbContext
        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

        // Disposes the DbContext when the UnitOfWork is no longer needed
        public void Dispose() => _db.Dispose();
    }
}