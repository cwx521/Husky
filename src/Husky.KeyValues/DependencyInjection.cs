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
				.AddDbContextPool<IKeyValueDbContext, KeyValueDbContext>(optionsAction)
				.AddScoped<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyInjector AddKeyValueManager<TDbContext>(this HuskyInjector husky)
			where TDbContext : DbContext, IKeyValueDbContext {
			husky.Services
				.AddDbContext<IKeyValueDbContext, TDbContext>()
				.AddScoped<IKeyValueManager, KeyValueManager>();
			return husky;
		}
	}
}