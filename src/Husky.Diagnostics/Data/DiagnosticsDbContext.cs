using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Husky.Diagnostics.Data
{
	public class DiagnosticsDbContext : DbContext
	{
		public DiagnosticsDbContext(DbContextOptions<DiagnosticsDbContext> options) : base(options) {
		}

		public DbSet<ExceptionLog> ExceptionLogs { get; set; } = null!;
		public DbSet<RequestLog> RequestLogs { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<ExceptionLog>().Property(x => x.FirstTime).HasDefaultValueSql("getdate()");
			modelBuilder.Entity<ExceptionLog>().Property(x => x.LastTime).HasDefaultValueSql("getdate()");
			modelBuilder.Entity<RequestLog>().Property(x => x.Time).HasDefaultValueSql("getdate()");

			modelBuilder.Entity<ExceptionLog>().HasIndex(x => x.Md5Comparison).IsUnique(false).IsClustered(false);
		}
	}
}
