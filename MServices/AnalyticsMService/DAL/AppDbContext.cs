using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<RegistrationStatistic> RegistrationStatistics { get; set; }
        public DbSet<LoginStatistic> LoginStatistics { get; set; }
        public DbSet<LogoutStatistic> LogoutStatistics { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAction>().UseTptMappingStrategy();

            modelBuilder.Entity<RegistrationStatistic>()
                .ToTable("RegistrationStatistics");
            modelBuilder.Entity<LoginStatistic>()
                .ToTable("LoginStatistics");
            modelBuilder.Entity<LogoutStatistic>()
                .ToTable("LogoutStatistics");

            ConfigureUserActionEntity(modelBuilder.Entity<UserAction>());

            modelBuilder = Configurations(modelBuilder);

            modelBuilder.Entity<RegistrationStatistic>()
                .HasIndex(r => r.UserId);

            modelBuilder.Entity<LoginStatistic>()
                .HasIndex(l => l.UserId);

            modelBuilder.Entity<LogoutStatistic>()
                .HasIndex(l => l.UserId);

            modelBuilder.Entity<RegistrationStatistic>()
                .HasIndex(r => r.UTC);

            modelBuilder.Entity<LoginStatistic>()
                .HasIndex(l => l.UTC);

            modelBuilder.Entity<LogoutStatistic>()
                .HasIndex(l => l.UTC);

            modelBuilder.Entity<LoginStatistic>()
                .HasIndex(l => new { l.UserId, l.UTC });
        }

        private ModelBuilder Configurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAction>()
                .HasKey(ua => ua.Id);

            modelBuilder.Entity<RegistrationStatistic>()
                .Property(r => r.Id);

            modelBuilder.Entity<LoginStatistic>()
                .Property(l => l.Id);

            modelBuilder.Entity<LogoutStatistic>()
                .Property(l => l.Id);

            modelBuilder.Entity<UserAction>()
                .Property(ua => ua.TimeZone)
                .HasMaxLength(100);

            modelBuilder.Entity<UserAction>()
                .Property(ua => ua.IpAddress)
                .HasMaxLength(45);

            modelBuilder.Entity<UserAction>()
                .Property(ua => ua.UserAgent)
                .HasMaxLength(1000);

            return modelBuilder;
        }

        private static void ConfigureUserActionEntity<T>(EntityTypeBuilder<T> builder)
            where T : UserAction
        {
            builder.Property(e => e.UTC)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            builder.Property(e => e.Local)
                .HasColumnType("timestamp without time zone")
                .HasConversion(
                    v => v.HasValue ? v.Value : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified) : (DateTime?)null);
        }
    }
}