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

		public static HuskyDI AddKeyValueManager<TImplementKeyValueDbContext>(this HuskyDI husky)
			where TImplementKeyValueDbContext : class, IKeyValueDbContext {
			husky.Services
				.AddScoped<IKeyValueDbContext, TImplementKeyValueDbContext>()
				.AddScoped<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyDI AddKeyValueManagerWithOwnImplement<TImplementKeyValueManager>(this HuskyDI husky)
			where TImplementKeyValueManager : class, IKeyValueManager {
			husky.Services.AddScoped<IKeyValueManager, TImplementKeyValueManager>();
			return husky;
		}
	}
}