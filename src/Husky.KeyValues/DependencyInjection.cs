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

		public static HuskyDI AddKeyValueManager<TImplementKeyValueDbContext>(this HuskyDI husky)
			where TImplementKeyValueDbContext : class, IKeyValueDbContext {
			husky.Services
				.AddScoped<IKeyValueDbContext, TImplementKeyValueDbContext>()
				.AddSingleton<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyDI AddKeyValueManagerWithOwnImplement<TImplementKeyValueManager>(this HuskyDI husky)
			where TImplementKeyValueManager : class, IKeyValueManager {
			husky.Services.AddSingleton<IKeyValueManager, TImplementKeyValueManager>();
			return husky;
		}
	}
}