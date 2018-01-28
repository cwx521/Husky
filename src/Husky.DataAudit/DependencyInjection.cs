using Husky.DataAudit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		public static HuskyDependencyInjectionHub AddDataAudit(this HuskyDependencyInjectionHub husky, string nameOfConnectionString = null) {
			husky.Services.AddDbContext<AuditDbContext>((svc, builder) => {
				var config = svc.GetRequiredService<IConfiguration>();
				var connstr = config.SeekConnectionStringSequence<AuditDbContext>(nameOfConnectionString);
				builder.UseSqlServer(connstr);
				builder.Migrate();
			});
			return husky;
		}
	}
}