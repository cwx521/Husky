using Husky.Data.Abstractions;
using Husky.Data.ModelBuilding;
using Microsoft.EntityFrameworkCore;

namespace Husky.Data
{
	public abstract class DbContextBase : DbContext
	{
		public DbContextBase(IDatabaseFinder connstr) {
			DatabaseFinder = connstr;
		}

		protected IDatabaseFinder DatabaseFinder { get; private set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			if ( !optionsBuilder.IsConfigured ) {
				optionsBuilder.UseSqlServer(DatabaseFinder.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ForSqlServer(GetType());
		}
	}
}
