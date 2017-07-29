using Husky.Authentication.Abstractions;
using Husky.TwoFactor.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.TwoFactor
{
	public static class ServiceRegisterTwoFactor
	{
		public static IServiceCollection AddHuskyTwoFactorPlugin(this IServiceCollection services) => services
			.AddDbContext<TwoFactorDbContext>()
			.AddSingleton<PrincipalTwoFactorExtensions>();

		public static PrincipalTwoFactorExtensions User<T>(this T principal) where T : IPrincipal {
			return principal.ServiceProvider.GetRequiredService<PrincipalTwoFactorExtensions>();
		}
	}
}