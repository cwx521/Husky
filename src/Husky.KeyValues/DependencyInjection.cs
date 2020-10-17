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

		public static HuskyDI AddKeyValueManager<TKeyValueDbContextImplement>(this HuskyDI husky)
			where TKeyValueDbContextImplement : class, IKeyValueDbContext {
			husky.Services
				.AddScoped<IKeyValueDbContext, TKeyValueDbContextImplement>()
				.AddScoped<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyDI AddKeyValueManagerWithOwnImplement<TKeyValueManagerImplement>(this HuskyDI husky)
			where TKeyValueManagerImplement : class, IKeyValueManager {
			husky.Services.AddScoped<IKeyValueManager, TKeyValueManagerImplement>();
			return husky;
		}
	}
}