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
				.AddSingleton<IKeyValueManager, KeyValueManager>();

			return husky;
		}

		public static HuskyDI AddKeyValueManager<TImplement>(this HuskyDI husky)
			where TImplement : class, IKeyValueDbContext {
			husky.Services
				.AddScoped<IKeyValueDbContext, TImplement>()
				.AddSingleton<IKeyValueManager, KeyValueManager>();

			return husky;
		}

		public static HuskyDI AddKeyValueManagerWithNoGivenDatabase<TImplement>(this HuskyDI husky)
			where TImplement : class, IKeyValueManager {
			husky.Services
				.AddSingleton<IKeyValueManager, TImplement>();

			return husky;
		}
	}
}