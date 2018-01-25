using Husky.AspNetCore.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore
{
	public static class DI
	{
		public static HuskyDependencyInjectionHub AddMail(this HuskyDependencyInjectionHub husky, string dbConnectionString = null) {
			husky.ServiceCollection
				.AddDbContext<TwoFactorDbContext>((svc, builder) => {
					builder.UseSqlServer(
						dbConnectionString ??
						svc.GetRequiredService<IConfiguration>().GetConnectionStringBySeekSequence<TwoFactorDbContext>()
					);
					builder.Migrate();
				});

			return husky;
		}
	}
}
