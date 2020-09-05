using Husky.DataAudit.Data;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddDataAudit(this HuskyDI husky, string nameOfConnectionString = null, bool migrateRequiredDatabase = true) {
			husky.Services.AddDbContextPool<AuditDbContext>(nameOfConnectionString, migrateRequiredDatabase);
			return husky;
		}
	}
}