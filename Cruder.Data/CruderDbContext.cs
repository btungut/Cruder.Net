using Cruder.Core.Configuration;
using Cruder.Data.Model;
using System.Data.Entity;

namespace Cruder.Data
{
    public sealed class CruderDbContext : DbContext
    {
        public DbSet<ConfigEntity> Configs { get; set; }
        public DbSet<LogEntity> Logs { get; set; }
        public DbSet<RouteEntity> Routes { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserGroupEntity> UserGroups { get; set; }
        public DbSet<UserGroupRouteMappingEntity> UserGroupRouteMappings { get; set; }

        public CruderDbContext()
            : base(System.Configuration.ConfigurationManager.ConnectionStrings[ConfigurationFactory.Application.ConnectionStringKey].ConnectionString)
        {
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            System.Data.Entity.Database.SetInitializer<CruderDbContext>(new CruderDbTableSeeder());

            modelBuilder.Entity<ConfigEntity>()
                .ToTable("Cruder.Configs");

            modelBuilder.Entity<LogEntity>()
                .ToTable("Cruder.Logs");

            modelBuilder.Entity<RouteEntity>()
                .ToTable("Cruder.Routes");

            modelBuilder.Entity<UserEntity>()
                .ToTable("Cruder.Users");

            modelBuilder.Entity<UserGroupEntity>()
                .ToTable("Cruder.UserGroups")
                .HasMany(g => g.Users)
                .WithRequired(u => u.UserGroup)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
