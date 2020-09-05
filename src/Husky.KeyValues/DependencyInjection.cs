using Husky.KeyValues;
using Husky.KeyValues.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddDiagnostics(this HuskyDI husky, string nameOfConnectionString = null, bool migrateRequiredDatabase = true) {
			husky.Services
				.AddDbContextPool<KeyValueDbContext>(nameOfConnectionString, migrateRequiredDatabase)
				.AddSingleton<KeyValueManager>();

			return husky;
		}
	}
}