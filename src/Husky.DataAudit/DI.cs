using Husky.DataAudit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DI
	{
		public static HuskyDependencyInjectionHub AddDataAudit(this HuskyDependencyInjectionHub husky, string dbConnectionString = null) {
			husky.ServiceCollection.AddDbContext<AuditDbContext>((svc, builder) => {
				builder.UseSqlServer(
					dbConnectionString ??
					svc.GetRequiredService<IConfiguration>().GetConnectionStringBySeekSequence<AuditDbContext>()
				);
				builder.Migrate();
			});
			return husky;
		}
	}
}