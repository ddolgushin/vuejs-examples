using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RunTogether.Dal.Model;

namespace RunTogether
{
	public partial class DatabaseContext : DbContext, IDesignTimeDbContextFactory<DatabaseContext>
	{
		private string _connectionString;

		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Marathon> Marathons { get; set; }
		public virtual DbSet<MarathonLink> MarathonLinks { get; set; }

		protected DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

		public DatabaseContext() { }

		public DatabaseContext(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentNullException(nameof(connectionString));

			_connectionString = connectionString;

			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
				optionsBuilder.UseNpgsql(_connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder
				.HasPostgresExtension("adminpack");

      modelBuilder.Entity<User>().ToTable("User");
      modelBuilder.Entity<Marathon>().ToTable("Marathon");
			modelBuilder.Entity<MarathonLink>().ToTable("MarathonLink");
			modelBuilder.Entity<WaypointInfo>().ToTable("WaypointInfo");

			modelBuilder.Entity<MarathonLink>(entity =>
			{
				entity.HasOne(o => o.User)
					.WithMany(o => o.MarathonLinks)
					.HasForeignKey(o => o.UserId)
					.OnDelete(DeleteBehavior.Cascade);
				entity.HasOne(o => o.Marathon)
					.WithMany(o => o.MarathonLinks)
					.HasForeignKey(o => o.MarathonId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<WaypointInfo>(entity =>
			{
				entity.HasOne(o => o.Marathon)
					.WithMany(o => o.WaypointInfos)
					.HasForeignKey(o => o.MarathonId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}

		public DatabaseContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
			optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

			return new DatabaseContext(optionsBuilder.Options);
		}
	}
}
