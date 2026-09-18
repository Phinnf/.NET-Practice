using CarRental.Modules.Cars.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars => Set<Car>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Brand)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.CarModel)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Color)
                    .HasMaxLength(50);

                entity.Property(c => c.BasePrice)
                    .HasColumnType("decimal(18,2)");

                // Optimistic Concurrency token mapped to PostgreSQL xmin
                entity.Property(c => c.Version)
                    .IsRowVersion();
            });
        }
    }
}
