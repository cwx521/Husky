using Husky.DataAudit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		private static bool migrated = false;

		public static HuskyDI AddDataAudit(this HuskyDI husky, string nameOfConnectionString = null) {
			husky.Services.AddDbContextPool<AuditDbContext>((svc, builder) => {
				var config = svc.GetRequiredService<IConfiguration>();
				var connstr = config.SeekConnectionString<AuditDbContext>(nameOfConnectionString);
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