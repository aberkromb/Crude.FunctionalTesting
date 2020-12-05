using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sandbox.DataAccess.DataBase
{
    public class PostgresDbContext : DbContext
    {
        private readonly ILogger<PostgresDbContext> _logger;
        private readonly PostgresOptions _options;

        public PostgresDbContext(
            IOptions<PostgresOptions> options,
            DbContextOptions<PostgresDbContext> dbContextOptions,
            ILogger<PostgresDbContext> logger
            ) : base(dbContextOptions)
        {
            _logger = logger;
            _options = options.Value;
            Database.EnsureCreated();
        }

        //Entities
        public DbSet<Strings> Strings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Strings>()
                .ToTable("strings")
                .HasKey(p => p.Id)
                .HasName("PK_Strings");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(
                _options.ConnectionString
                /*"Host=localhost; Port=5432; Database=postgres; Username=postgres; Password=mystrongpassword"*/);
    }
}