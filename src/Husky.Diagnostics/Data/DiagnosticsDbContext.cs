#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public class DiagnosticsDbContext : DbContext, IDiagnosticsDbContext
	{
		public DiagnosticsDbContext(DbContextOptions<DiagnosticsDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<ExceptionLog> ExceptionLogs { get; set; }
		public DbSet<RequestLog> RequestLogs { get; set; }
		public DbSet<OperationLog> OperationLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyAdditionalCustomAnnotations(this);
		}
	}
}
