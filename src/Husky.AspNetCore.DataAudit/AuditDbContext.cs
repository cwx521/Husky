using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Husky.AspNetCore.DataAudit
{
	public sealed class AuditDbContext : DbContext
	{
		public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) {
		}

		public DbSet<AuditEntry> AuditEntries { get; set; }
		public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }
	}
}
