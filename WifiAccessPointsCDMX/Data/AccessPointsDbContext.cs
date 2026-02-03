using WifiAccessPointsCDMX.Models;
using Microsoft.EntityFrameworkCore;

namespace WifiAccessPointsCDMX.Data
{
    public class AccessPointsDbContext : DbContext
    {
        public AccessPointsDbContext(DbContextOptions<AccessPointsDbContext> options) : base(options) { }

        public DbSet<ProgramModel> Programs => Set<ProgramModel>();
        public DbSet<AlcaldiaModel> Alcaldias => Set<AlcaldiaModel>();
        public DbSet<AccessPointModel> AccessPoints => Set<AccessPointModel>();

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