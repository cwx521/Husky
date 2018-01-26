using Husky.AspNetCore.Diagnostics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore.DependencyInjection
{
	public static class DI
	{
		public static HuskyDependencyInjectionHub AddDiagnostics(this HuskyDependencyInjectionHub husky, string dbConnectionString = null) {
			husky.ServiceCollection.AddDbContext<DiagnosticsDbContext>((svc, builder) => {
				builder.UseSqlServer(
					dbConnectionString ??
					svc.GetRequiredService<IConfiguration>().GetConnectionStringBySeekSequence<DiagnosticsDbContext>()
				);
				builder.Migrate();
			});
			return husky;
		}
	}
}