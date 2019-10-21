using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public class DiagnosticsDbContext : DbContext
	{
		public DiagnosticsDbContext(DbContextOptions<DiagnosticsDbContext> options) : base(options) {
		}

		public DbSet<ExceptionLog> ExceptionLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<ExceptionLog>().HasIndex(x => x.Md5Comparison).IsUnique(false).IsClustered(false);
		}
	}
}
