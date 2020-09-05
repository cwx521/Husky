using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore.Internal;

namespace Husky
{
	public class HuskyDI
	{
		internal HuskyDI(IServiceCollection services) {
			Services = services;
		}
		public IServiceCollection Services { get; private set; }
	}

	public static class HuskyDependencyInjectionHelper
	{
		static readonly List<Type> _migrated = new List<Type>();

		public static HuskyDI Husky(this IServiceCollection services)
			=> new HuskyDI(services);

		public static IServiceCollection AddDbContextPool<TContext>(this IServiceCollection services, string nameOfConnectionString = null, bool migrate = true) where TContext : DbContext {
			return services.AddDbContextPool<TContext>((svc, builder) => {
				var config = svc.GetRequiredService<IConfiguration>();
				var connstr = config.SeekConnectionString<TContext>(nameOfConnectionString);
				builder.UseSqlServer(connstr);

				if ( migrate && !_migrated.Contains(typeof(TContext)) ) {
					builder.CreateDbContext().Database.Migrate();
					_migrated.Add(typeof(TContext));
				}
			});
		}
	}
}
