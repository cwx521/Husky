using System;
using Husky.Authentication.Abstractions;
using Husky.Users.Data;
using Husky.Users.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Users
{
	public static class ServiceRegisterUsers
	{
		public static IServiceCollection AddHuskyUsersModule(this IServiceCollection services, Action<DbContextOptionsBuilder> databaseOptions) {
			return services
				.AddDbContext<UserDbContext>(databaseOptions)
				.AddSingleton<PrincipalUserExtensions>();
		}

		public static PrincipalUserExtensions User<T>(this T principal) where T : IPrincipal {
			return principal.ServiceProvider.GetRequiredService<PrincipalUserExtensions>();
		}
	}
}