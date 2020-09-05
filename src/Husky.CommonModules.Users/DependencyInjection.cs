using Husky.CommonModules.Users.Data;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddUserModule(this HuskyDI husky, string? nameOfConnectionString = null, bool migrateRequiredDatabase = true) {
			husky.Services.AddDbContextPool<UserModuleDbContext>(nameOfConnectionString, migrateRequiredDatabase);
			return husky;
		}
	}
}
