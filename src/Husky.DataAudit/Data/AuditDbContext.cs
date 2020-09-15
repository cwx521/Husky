using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Husky.DataAudit.Data
{
	public sealed class AuditDbContext : DbContext
	{
		public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) {
		}

		internal AuditDbContext(DbContextOptions options) : base(options) {
		}

		public DbSet<AuditEntry> AuditEntries { get; set; } = null!;
		public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; } = null!;
	}
}
