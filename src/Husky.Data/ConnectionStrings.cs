using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Husky.Data
{
	public static class ConnectionStrings
	{
		public static string GetConnectionStringBySequence<T>(this IConfiguration configuration) where T : DbContext {
			var lookFor = new[] { "Debugging", typeof(T).Name.Replace("DbContext", ""), "Default" };
			var connstr = lookFor.Select(x => configuration.GetConnectionString(x)).FirstOrDefault(x => !string.IsNullOrEmpty(x));
			if ( string.IsNullOrEmpty(connstr) ) {
				throw new Exception("Didn't find any applicable ConnectionString, these ConnectionString names are searched: " + string.Join(", ", lookFor));
			}
			return connstr;
		}

		public static string LocalIntegratedSecurityConnectionString(string databaseName) {
			if ( string.IsNullOrWhiteSpace(databaseName) ) {
				throw new ArgumentNullException(nameof(databaseName));
			}
			return $"Data Source=localhost;Initial Catalog={databaseName};Integrated Security=True";
		}

		public static string LocalIntegratedSecurityConnectionString<T>() where T : DbContext {
			return LocalIntegratedSecurityConnectionString(nameof(T));
		}
	}
}
