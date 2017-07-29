using Husky.Authentication.Abstractions;
using Husky.Users.Data;
using Husky.Users.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Users
{
	public static class ServiceRegisterUsers
	{
		public static IServiceCollection AddHuskyUsersPlugin(this IServiceCollection services) => services
			.AddDbContext<UserDbContext>()
			.AddSingleton<PrincipalUserExtensions>();

		public static PrincipalUserExtensions User<T>(this T principal) where T : IPrincipal {
			return principal.ServiceProvider.GetRequiredService<PrincipalUserExtensions>();
		}
	}
}