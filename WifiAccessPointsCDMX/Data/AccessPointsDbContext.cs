using WifiAccessPointsCDMX.Models;
using Microsoft.EntityFrameworkCore;

namespace WifiAccessPointsCDMX.Data
{
    public class AccessPointsDbContext : DbContext
    {
        public AccessPointsDbContext(DbContextOptions<AccessPointsDbContext> options) : base(options) { }

        // Exposes the models tables as a DbSet so EF Core can query, insert, update, and delete Program records
        public DbSet<ProgramModel> Programs => Set<ProgramModel>();
        public DbSet<AlcaldiaModel> Alcaldias => Set<AlcaldiaModel>();
        public DbSet<AccessPointModel> AccessPoints => Set<AccessPointModel>();

        // Configure EF Core model mappings and database schema details
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlcaldiaModel>()
                .ToTable("Alcaldias", "cat")
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<ProgramModel>()
            .ToTable("Programs")
            .HasIndex(p => p.Name)
            .IsUnique();

            modelBuilder.Entity<AccessPointModel>(entity =>
            {
                entity.ToTable("AccessPoints");

                entity.HasKey(a => a.Id);

                entity.HasIndex(a => a.Code)
                      .IsUnique();
            });
        }
    }
}