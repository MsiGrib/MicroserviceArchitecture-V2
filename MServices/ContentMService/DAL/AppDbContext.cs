using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder = Configurations(modelBuilder);

            modelBuilder.Entity<Reaction>()
                .HasIndex(r => new { r.UserId, r.PostId })
                .IsUnique();
        }

        private ModelBuilder Configurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(p => p.Content)
                    .IsRequired()
                    .HasColumnType("text");
                entity.Property(p => p.CreatedAt)
                    .IsRequired();
                entity.Property(p => p.UpdatedAt)
                    .IsRequired(false);
                entity.Property(p => p.DeletedAt)
                    .IsRequired(false);
                entity.Property(p => p.UserId)
                    .IsRequired();

                entity.HasQueryFilter(p => p.DeletedAt == null);

                entity.HasMany(p => p.Comments)
                    .WithOne(c => c.Post)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(p => p.Reactions)
                    .WithOne(r => r.Post)
                    .HasForeignKey(r => r.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Text)
                    .IsRequired()
                    .HasMaxLength(2000);
                entity.Property(c => c.UserId)
                    .IsRequired();
                entity.Property(c => c.PostId)
                    .IsRequired();

                entity.HasIndex(c => c.PostId);
                entity.HasIndex(c => c.UserId);
            });

            modelBuilder.Entity<Reaction>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Type)
                    .IsRequired();
                entity.Property(r => r.UserId)
                    .IsRequired();
                entity.Property(r => r.PostId)
                    .IsRequired();

                entity.HasIndex(r => r.PostId);
                entity.HasIndex(r => r.UserId);

                entity.HasOne(r => r.Post)
                    .WithMany(p => p.Reactions)
                    .HasForeignKey(r => r.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            return modelBuilder;
        }
    }
}