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
				.AddSingleton<KeyValueManager>();

			return husky;
		}
	}
}