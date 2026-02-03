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

        public Task<ProgramModel?> GetByNameAsync(string name) =>
            _db.Programs.FirstOrDefaultAsync(p => p.Name == name);

        public async Task AddAsync(ProgramModel program)
        {
            _db.Programs.Add(program);
            await _db.SaveChangesAsync();
        }

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

            bulk.ColumnMappings.Add("Name", "Name");

            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));

            foreach (var p in items)
                table.Rows.Add(p.Name);

            await bulk.WriteToServerAsync(table);
        }
    }
}