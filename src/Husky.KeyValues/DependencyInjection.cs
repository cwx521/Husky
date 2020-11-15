using System;
using Husky.KeyValues;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddKeyValueManager(this HuskyServiceHub husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<IKeyValueDbContext, KeyValueDbContext>(optionsAction);
			husky.Services.AddSingleton<IKeyValueManager, KeyValueManager>();
			return husky;
		}

		public static HuskyServiceHub AddKeyValueManager<TDbContext>(this HuskyServiceHub husky)
			where TDbContext : DbContext, IKeyValueDbContext {
			husky.Services.AddDbContext<IKeyValueDbContext, TDbContext>();
			husky.Services.AddSingleton<IKeyValueManager, KeyValueManager>();
			return husky;
		}
	}
}