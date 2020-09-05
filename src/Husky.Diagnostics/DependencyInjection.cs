using Husky.Diagnostics.Data;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddDiagnostics(this HuskyDI husky, string nameOfConnectionString = null, bool migrateRequiredDatabase = true) {
			husky.Services.AddDbContextPool<DiagnosticsDbContext>(nameOfConnectionString, migrateRequiredDatabase);
			return husky;
		}
	}
}