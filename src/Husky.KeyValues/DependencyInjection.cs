using Husky.KeyValues;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		private static bool migrated = false;

		public static HuskyDependencyInjectionHub KeyValueManager(this HuskyDependencyInjectionHub husky, string nameOfConnectionString = null) {
			husky.Services
				.AddDbContextPool<KeyValueDbContext>((svc, builder) => {
					var config = svc.GetRequiredService<IConfiguration>();
					var connstr = config.SeekConnectionString<KeyValueDbContext>(nameOfConnectionString);
					builder.UseSqlServer(connstr);

					if ( !migrated ) {
						builder.Migrate();
						migrated = true;
					}
				})
				.AddSingleton<KeyValueManager>();

			return husky;
		}
	}
}