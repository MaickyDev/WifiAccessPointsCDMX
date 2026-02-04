using System.Data;
using Microsoft.Data.SqlClient;
using WifiAccessPointsCDMX.Data;
using WifiAccessPointsCDMX.Models;
using Microsoft.EntityFrameworkCore;
using WifiAccessPointsCDMX.Interfaces.Repositories;

namespace WifiAccessPointsCDMX.Repositories
{
    public class AccessPointRepository : IAccessPointRepository
    {
        private readonly AccessPointsDbContext _db;

        public AccessPointRepository(AccessPointsDbContext db)
        {
            _db = db;
        }

        public Task<List<string>> GetAllCodesAsync() =>
            _db.AccessPoints.Select(a => a.Code).ToListAsync();

        public async Task BulkInsertAsync(List<AccessPointModel> items)
        {
            if (items.Count == 0)
                return;

            using var connection = new SqlConnection(_db.Database.GetConnectionString());
            await connection.OpenAsync();

            using var bulk = new SqlBulkCopy(connection)
            {
                DestinationTableName = "AccessPoints"
            };

            // Map .NET properties to SQL columns
            bulk.ColumnMappings.Add("Code", "Code");
            bulk.ColumnMappings.Add("Latitude", "Latitude");
            bulk.ColumnMappings.Add("Longitude", "Longitude");
            bulk.ColumnMappings.Add("ProgramId", "ProgramId");
            bulk.ColumnMappings.Add("AlcaldiaId", "AlcaldiaId");

            // Build a DataTable that matches the SQL schema
            var table = new DataTable();
            table.Columns.Add("Code", typeof(string));
            table.Columns.Add("Latitude", typeof(double));
            table.Columns.Add("Longitude", typeof(double));
            table.Columns.Add("ProgramId", typeof(int));
            table.Columns.Add("AlcaldiaId", typeof(int));

            // Load rows into the DataTable
            foreach (var ap in items)
            {
                table.Rows.Add(ap.Code, ap.Latitude, ap.Longitude, ap.ProgramId, ap.AlcaldiaId);
            }

            await bulk.WriteToServerAsync(table);
        }

        // Load related Program and Alcaldia entities, apply pagination and execute the query
        public Task<int> CountAsync() =>
            _db.AccessPoints.CountAsync();

        public Task<int> CountByAlcaldiaAsync(int alcaldiaId) =>
            _db.AccessPoints.CountAsync(a => a.AlcaldiaId == alcaldiaId);

        public Task<List<AccessPointModel>> GetPagedAsync(int skip, int take) =>
            _db.AccessPoints.Include(a => a.Program).Include(a => a.Alcaldia).Skip(skip).Take(take).ToListAsync();

        public Task<List<AccessPointModel>> GetPagedByAlcaldiaAsync(int alcaldiaId, int skip, int take) =>
            _db.AccessPoints.Include(a => a.Program).Include(a => a.Alcaldia).Where(a => a.AlcaldiaId == alcaldiaId).Skip(skip).Take(take).ToListAsync();

        public Task<AccessPointModel?> GetByExternalIdAsync(string externalId) =>
            _db.AccessPoints.Include(a => a.Program).Include(a => a.Alcaldia).FirstOrDefaultAsync(a => a.Code == externalId);

        public Task<List<AccessPointModel>> GetNearbyPagedAsync(double lat, double lng, int skip, int take)
        {
            return _db.AccessPoints
                .Include(a => a.Program)
                .Include(a => a.Alcaldia)
                .OrderBy(a =>
                    (a.Latitude - lat) * (a.Latitude - lat) +
                    (a.Longitude - lng) * (a.Longitude - lng)
                )
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}