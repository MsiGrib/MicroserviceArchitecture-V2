using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder = Configurations(modelBuilder);
        }

        private ModelBuilder Configurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.HasIndex(e => e.Status);

                entity.HasIndex(e => e.CreatedAt);

                entity.HasIndex(e => new { e.Status, e.CreatedAt });
            });

            return modelBuilder;
        }
    }
}