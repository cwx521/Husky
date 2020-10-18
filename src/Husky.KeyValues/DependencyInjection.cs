using System;
using Husky.KeyValues;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddKeyValueManager(this HuskyInjector husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<KeyValueDbContext>(optionsAction)
				.AddScoped<IKeyValueDbContext, KeyValueDbContext>()
				.AddScoped<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyInjector AddKeyValueManager<TDbContext>(this HuskyInjector husky)
			where TDbContext : DbContext, IKeyValueDbContext {
			husky.Services
				.AddScoped<IKeyValueDbContext, TDbContext>()
				.AddScoped<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyInjector MapKeyValueManager<TKeyValueManagerImplement>(this HuskyInjector husky)
			where TKeyValueManagerImplement : class, IKeyValueManager {
			husky.Services.AddScoped<IKeyValueManager, TKeyValueManagerImplement>();
			return husky;
		}

		public static HuskyInjector MapKeyValueManager<TKeyValueManagerImplement>(this HuskyInjector husky, Func<IServiceProvider, TKeyValueManagerImplement> implementationFactory)
			where TKeyValueManagerImplement : class, IKeyValueManager {
			husky.Services.AddScoped<IKeyValueManager, TKeyValueManagerImplement>(implementationFactory);
			return husky;
		}
	}
}