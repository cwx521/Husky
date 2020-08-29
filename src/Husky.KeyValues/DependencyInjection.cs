using Husky.KeyValues;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		private static bool migrated = false;

		public static HuskyDI AddKeyValueManager(this HuskyDI husky, string nameOfConnectionString = null) {
			husky.Services
				.AddDbContextPool<KeyValueDbContext>((svc, builder) => {
					var config = svc.GetRequiredService<IConfiguration>();
					var connstr = config.SeekConnectionString<KeyValueDbContext>(nameOfConnectionString);
					builder.UseSqlServer(connstr);

					if ( !migrated ) {
						builder.CreateDbContext().Database.Migrate();
						migrated = true;
					}
				})
				.AddSingleton<KeyValueManager>();

			return husky;
		}
	}
}