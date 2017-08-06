using Husky.Authentication.Abstractions;
using Husky.Data;
using Husky.Users;
using Husky.Users.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Injection
{
	public static class RegSvcUsers
	{
		public static IServiceCollection AddHuskyUsersPlugin(this IServiceCollection services, string dbConnectionString = null) {
			services.AddDbContext<UserDbContext>((svc, builder) => {
				builder.UseSqlServer(dbConnectionString ?? svc.GetRequiredService<IConfiguration>().GetConnectionStringBySeekSequence<UserDbContext>());
				builder.Migrate();
			});
			services.AddSingleton<PrincipalUserExtensions>();
			return services;
		}

		public static PrincipalUserExtensions User<T>(this T principal) where T : IPrincipal {
			return principal.ServiceProvider.GetRequiredService<PrincipalUserExtensions>();
		}
	}
}