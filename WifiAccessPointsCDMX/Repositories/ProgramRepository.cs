using System.Data;
using Microsoft.Data.SqlClient;
using WifiAccessPointsCDMX.Data;
using WifiAccessPointsCDMX.Models;
using Microsoft.EntityFrameworkCore;
using WifiAccessPointsCDMX.Interfaces.Repositories;

namespace WifiAccessPointsCDMX.Repositories
{
    public class ProgramRepository : IProgramRepository
    {
        private readonly AccessPointsDbContext _db;

        public ProgramRepository(AccessPointsDbContext db)
        {
            _db = db;
        }

        public Task<List<ProgramModel>> GetAllAsync() =>
            _db.Programs.ToListAsync();

        public async Task BulkInsertAsync(List<ProgramModel> items)
        {
            if (items.Count == 0)
                return;

            using var connection = new SqlConnection(_db.Database.GetConnectionString());
            await connection.OpenAsync();

            using var bulk = new SqlBulkCopy(connection)
            {
                DestinationTableName = "Programs"
            };

            // Map .NET properties to SQL columns
            bulk.ColumnMappings.Add("Name", "Name");

            // Build a DataTable that matches the SQL schema
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));

            // Load rows into the DataTable
            foreach (var p in items)
                table.Rows.Add(p.Name);

            await bulk.WriteToServerAsync(table);
        }
    }
}