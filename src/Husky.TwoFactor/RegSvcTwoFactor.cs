using Husky.Authentication.Abstractions;
using Husky.Data;
using Husky.TwoFactor.Data;
using Husky.TwoFactor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Injection
{
	public static class RegSvcTwoFactor
	{
		public static IServiceCollection AddHuskyTwoFactorPlugin(this IServiceCollection services, string dbConnectionString = null) {
			services.AddDbContext<TwoFactorDbContext>((svc, builder) => {
				builder.UseSqlServer(dbConnectionString ?? svc.GetRequiredService<IConfiguration>().GetConnectionStringBySequence<TwoFactorDbContext>());
				builder.Migrate();
			});
			services.AddSingleton<PrincipalTwoFactorExtensions>();
			return services;
		}

		public static PrincipalTwoFactorExtensions TwoFactor<T>(this T principal) where T : IPrincipal {
			return principal.ServiceProvider.GetRequiredService<PrincipalTwoFactorExtensions>();
		}
	}
}