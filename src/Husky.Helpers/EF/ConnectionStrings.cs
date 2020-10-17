using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Husky
{
	public static class ConnectionStrings
	{
		public static string LookFor<TContext>(this IConfiguration configuration, string? nameOfConnectionString = null)
			where TContext : DbContext {

			//try looking for the first found connection string by this sequence

			var lookForNames = new[] {
				nameOfConnectionString,
				"Dev",
				"Test",
				typeof(TContext).Name.Replace("DbContext", ""),
				"Default"
			};
			var connstr = lookForNames
				.Select(name => configuration.GetConnectionString(name))
				.Where(value => !string.IsNullOrEmpty(value))
				.FirstOrDefault();

			if ( string.IsNullOrEmpty(connstr) ) {
				throw new Exception(
					"Didn't find any applicable ConnectionString in appSettings.json configuration, " +
					"these ConnectionString names have been tried to look for: " + string.Join(", ", lookForNames)
				);
			}
			return connstr;
		}
	}
}
