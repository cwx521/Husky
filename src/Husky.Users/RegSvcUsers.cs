using Husky.Authentication.Abstractions;
using Husky.Data.Abstractions;
using Husky.Users.Data;
using Husky.Users.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Injection
{
	public static class RegSvcUsers
	{
		public static IServiceCollection AddHuskyUsersPlugin(this IServiceCollection services, IDatabaseFinder database = null) => services
			.AddScoped(svc => new UserDbContext(database ?? svc.GetService<IDatabaseFinder>()))
			.AddSingleton<PrincipalUserExtensions>();

		public static PrincipalUserExtensions User<T>(this T principal) where T : IPrincipal {
			return principal.ServiceProvider.GetRequiredService<PrincipalUserExtensions>();
		}
	}
}