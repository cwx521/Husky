using Husky.TwoFactor;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		public static HuskyDependencyInjectionHub AddTwoFactor(this HuskyDependencyInjectionHub husky, string nameOfConnectionString = null) {
			husky.Services
				.AddDbContext<TwoFactorDbContext>((svc, builder) => {
					var config = svc.GetRequiredService<IConfiguration>();
					var connstr = config.SeekConnectionStringSequence<TwoFactorDbContext>(nameOfConnectionString);
					builder.UseSqlServer(connstr);
					builder.Migrate();
				})
				.AddScoped<TwoFactorManager>();

			return husky;
		}
	}
}
