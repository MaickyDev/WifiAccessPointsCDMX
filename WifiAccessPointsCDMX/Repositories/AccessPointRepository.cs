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

            // Explicit column mappings
            bulk.ColumnMappings.Add("Code", "Code");
            bulk.ColumnMappings.Add("Latitude", "Latitude");
            bulk.ColumnMappings.Add("Longitude", "Longitude");
            bulk.ColumnMappings.Add("ProgramId", "ProgramId");
            bulk.ColumnMappings.Add("AlcaldiaId", "AlcaldiaId");

            var table = new DataTable();
            table.Columns.Add("Code", typeof(string));
            table.Columns.Add("Latitude", typeof(double));
            table.Columns.Add("Longitude", typeof(double));
            table.Columns.Add("ProgramId", typeof(int));
            table.Columns.Add("AlcaldiaId", typeof(int));


            foreach (var ap in items)
            {
                if (ap.AlcaldiaId == 0)
                {
                    throw new Exception($"AlcaldiaId is missing for Code={ap.Code}");
                }
                else
                {
                    table.Rows.Add(ap.Code, ap.Latitude, ap.Longitude, ap.ProgramId, ap.AlcaldiaId);
                }
            }

            await bulk.WriteToServerAsync(table);
        }

    }
}