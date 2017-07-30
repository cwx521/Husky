using Husky.Authentication.Abstractions;
using Husky.Data.Abstractions;
using Husky.TwoFactor.Data;
using Husky.TwoFactor.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Injection
{
	public static class RegSvcTwoFactor
	{
		public static IServiceCollection AddHuskyTwoFactorPlugin(this IServiceCollection services, IDatabaseFinder database = null) => services
			.AddScoped(svc => new TwoFactorDbContext(database ?? svc.GetService<IDatabaseFinder>()))
			.AddSingleton<PrincipalTwoFactorExtensions>();

		public static PrincipalTwoFactorExtensions TwoFactor<T>(this T principal) where T : IPrincipal {
			return principal.ServiceProvider.GetRequiredService<PrincipalTwoFactorExtensions>();
		}
	}
}