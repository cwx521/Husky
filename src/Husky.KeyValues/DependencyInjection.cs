using System;
using Husky.KeyValues;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddKeyValueManager(this HuskyDI husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<KeyValueDbContext>(optionsAction)
				.AddScoped<IKeyValueDbContext, KeyValueDbContext>()
				.AddScoped<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyDI AddKeyValueManager<TDbContext>(this HuskyDI husky)
			where TDbContext : DbContext, IKeyValueDbContext {
			husky.Services
				.AddScoped<IKeyValueDbContext, TDbContext>()
				.AddScoped<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyDI MapKeyValueManager<TKeyValueManagerImplement>(this HuskyDI husky)
			where TKeyValueManagerImplement : class, IKeyValueManager {
			husky.Services.AddScoped<IKeyValueManager, TKeyValueManagerImplement>();
			return husky;
		}

		public static HuskyDI MapKeyValueManager<TKeyValueManagerImplement>(this HuskyDI husky, Func<IServiceProvider, TKeyValueManagerImplement> implementationFactory)
			where TKeyValueManagerImplement : class, IKeyValueManager {
			husky.Services.AddScoped<IKeyValueManager, TKeyValueManagerImplement>(implementationFactory);
			return husky;
		}
	}
}