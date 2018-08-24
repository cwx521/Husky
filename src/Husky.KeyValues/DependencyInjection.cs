﻿using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.DependencyInjection
{
	public static class DependencyInjection
	{
		private static bool migrated = false;

		public static HuskyDependencyInjectionHub AddConfiguration(this HuskyDependencyInjectionHub husky, string nameOfConnectionString = null) {
			husky.Services.AddDbContext<KeyValueDbContext>((svc, builder) => {
				var config = svc.GetRequiredService<IConfiguration>();
				var connstr = config.SeekConnectionStringSequence<KeyValueDbContext>(nameOfConnectionString);
				builder.UseSqlServer(connstr);

				if ( !migrated ) {
					builder.Migrate();
					migrated = true;
				}
			});
			return husky;
		}
	}
}