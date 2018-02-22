using Husky.Diagnostics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		static bool migrated = false;

		public static HuskyDependencyInjectionHub AddDiagnostics(this HuskyDependencyInjectionHub husky, string nameOfConnectionString = null) {
			husky.Services.AddDbContext<DiagnosticsDbContext>((svc, builder) => {
				var config = svc.GetRequiredService<IConfiguration>();
				var connstr = config.SeekConnectionStringSequence<DiagnosticsDbContext>(nameOfConnectionString);
				builder.UseSqlServer(connstr);

				if ( !migrated ) {
					builder.Migrate();
					migrated = true;
				}
			});
			return husky;
		}
	}
}