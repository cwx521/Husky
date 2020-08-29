using Microsoft.EntityFrameworkCore;

namespace Husky.KeyValues.Data
{
	public class KeyValueDbContext : DbContext
	{
		public KeyValueDbContext(DbContextOptions<KeyValueDbContext> options) : base(options) {
		}

		public DbSet<KeyValue> KeyValues { get; set; }
	}
}
