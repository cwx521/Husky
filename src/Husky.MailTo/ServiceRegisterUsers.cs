using System;
using Husky.MailTo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.MailTo
{
	public static class ServiceRegisterUsers
	{
		public static IServiceCollection AddHuskyUsersModule(this IServiceCollection services, Action<DbContextOptionsBuilder> databaseOptions) {
			return services
				.AddDbContext<MailToDbContext>(databaseOptions);
		}
	}
}