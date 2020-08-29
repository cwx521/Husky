﻿using Husky.TwoFactor;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		private static bool migrated = false;

		public static HuskyDI AddTwoFactor(this HuskyDI husky, string nameOfConnectionString = null) {
			husky.Services
				.AddDbContextPool<TwoFactorDbContext>((svc, builder) => {
					var config = svc.GetRequiredService<IConfiguration>();
					var connstr = config.SeekConnectionString<TwoFactorDbContext>(nameOfConnectionString);
					builder.UseSqlServer(connstr);

					if ( !migrated ) {
						builder.CreateDbContext().Database.Migrate();
						migrated = true;
					}
				})
				.AddScoped<TwoFactorManager>();

			return husky;
		}
	}
}
