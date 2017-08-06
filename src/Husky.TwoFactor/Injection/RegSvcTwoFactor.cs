using Husky.Authentication.Abstractions;
using Husky.Data;
using Husky.TwoFactor;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Injection
{
	public static class RegSvcTwoFactor
	{
		public static IServiceCollection AddHuskyTwoFactorPlugin(this IServiceCollection services, string dbConnectionString = null) {
			services.AddDbContext<TwoFactorDbContext>((svc, builder) => {
				builder.UseSqlServer(dbConnectionString ?? svc.GetRequiredService<IConfiguration>().GetConnectionStringBySeekSequence<TwoFactorDbContext>());
				builder.Migrate();
			});
			services.AddSingleton<TwoFactorManager>();
			return services;
		}

		public static TwoFactorManager TwoFactor<T>(this T principal) where T : IPrincipal {
			return principal.ServiceProvider.GetRequiredService<TwoFactorManager>();
		}
	}
}