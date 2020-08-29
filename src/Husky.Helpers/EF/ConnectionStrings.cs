using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Husky
{
	public static class ConnectionStrings
	{
		public static string SeekConnectionString<T>(this IConfiguration configuration, string nameOfConnectionString = null)
			where T : DbContext {

			var lookForNames = new[] {
				nameOfConnectionString,
				"Dev",
				"QA",
				typeof(T).Name.Replace("DbContext", ""),
				"Default"
			};
			var connstr = lookForNames
				.Select(name => configuration.GetConnectionString(name))
				.Where(value => !string.IsNullOrEmpty(value))
				.FirstOrDefault();

			if ( string.IsNullOrEmpty(connstr) ) {
				throw new Exception(
					"Didn't find any applicable ConnectionString in appSettings.json configuration, " +
					"these ConnectionString names have been tried: " + string.Join(", ", lookForNames)
				);
			}
			return connstr;
		}
	}
}
