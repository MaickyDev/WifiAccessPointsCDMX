using System.Data;
using Microsoft.Data.SqlClient;
using WifiAccessPointsCDMX.Data;
using WifiAccessPointsCDMX.Models;
using Microsoft.EntityFrameworkCore;
using WifiAccessPointsCDMX.Interfaces.Repositories;

namespace WifiAccessPointsCDMX.Repositories
{
    public class AlcaldiaRepository : IAlcaldiaRepository
    {
        private readonly AccessPointsDbContext _db;

        public AlcaldiaRepository(AccessPointsDbContext db)
        {
            _db = db;
        }

        public Task<List<AlcaldiaModel>> GetAllAsync() =>
            _db.Alcaldias.ToListAsync();

        public Task<AlcaldiaModel?> GetByNameAsync(string name) =>
            _db.Alcaldias.FirstOrDefaultAsync(a => a.Name == name);

        public async Task AddAsync(AlcaldiaModel alcaldia)
        {
            _db.Alcaldias.Add(alcaldia);
            await _db.SaveChangesAsync();
        }

        public async Task BulkInsertAsync(List<AlcaldiaModel> items)
        {
            if (items.Count == 0)
                return;

            using var connection = new SqlConnection(_db.Database.GetConnectionString());
            await connection.OpenAsync();

            using var bulk = new SqlBulkCopy(connection)
            {
                DestinationTableName = "cat.Alcaldias"
            };

            bulk.ColumnMappings.Add("Name", "Name");

            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));

            foreach (var a in items)
                table.Rows.Add(a.Name);

            await bulk.WriteToServerAsync(table);
        }
    }
}