using Husky.Diagnostics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		private static bool migrated = false;

		public static HuskyDI AddDiagnostics(this HuskyDI husky, string nameOfConnectionString = null) {
			husky.Services.AddDbContextPool<DiagnosticsDbContext>((svc, builder) => {
				var config = svc.GetRequiredService<IConfiguration>();
				var connstr = config.SeekConnectionString<DiagnosticsDbContext>(nameOfConnectionString);
				builder.UseSqlServer(connstr);

				if ( !migrated ) {
					builder.CreateDbContext().Database.Migrate();
					migrated = true;
				}
			});
			return husky;
		}
	}
}