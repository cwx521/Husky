using System;
using Husky.Data.Abstractions;

namespace Husky.Data
{
	public class DatabaseFinder : IDatabaseFinder
	{
		public DatabaseFinder() {
			Provider = DatabaseProvider.SqlServer;
			ConnectionString = LocalIntegratedSecurityConnectionString("Husky");
		}

		public DatabaseFinder(DatabaseProvider provider, string connectionString) {
			Provider = provider;
			ConnectionString = connectionString;
		}

		public DatabaseProvider Provider { get; set; }
		public string ConnectionString { get; set; }

		public static string LocalIntegratedSecurityConnectionString(string databaseName) {
			if ( string.IsNullOrWhiteSpace(databaseName) ) {
				throw new ArgumentNullException(nameof(databaseName));
			}
			return $"Data Source=localhost;Initial Catalog={databaseName};Integrated Security=True";
		}
	}
}