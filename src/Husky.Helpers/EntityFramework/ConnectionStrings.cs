using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Husky
{
	public static class ConnectionStrings
	{
		public static string SeekConnectionString<TDbContext>(this IConfiguration configuration, string? nameOfConnectionString = null)
			where TDbContext : DbContext {
			return SeekConnectionString(configuration, nameOfConnectionString ?? typeof(TDbContext).Name.Replace("DbContext", ""));
		}

		public static string SeekConnectionString(this IConfiguration configuration, string? nameOfConnectionString = null) {
			var lookForNames = new[] {
				nameOfConnectionString,
				"Dev",
				"Test",
				"Default"
			};
			var connstr = lookForNames
				.Where(name => !string.IsNullOrEmpty(name))
				.Select(name => configuration.GetConnectionString(name!))
				.Where(value => !string.IsNullOrEmpty(value))
				.FirstOrDefault();

			if (string.IsNullOrEmpty(connstr)) {
				throw new Exception(
					"Didn't find any applicable ConnectionString in appSettings.json configuration, " +
					"these ConnectionString names have been tried to look for: " + string.Join(", ", lookForNames)
				);
			}
			return connstr;
		}
	}
}
