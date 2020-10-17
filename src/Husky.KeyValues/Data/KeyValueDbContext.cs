using Microsoft.EntityFrameworkCore;

namespace Husky.KeyValues.Data
{
	public class KeyValueDbContext : DbContext, IKeyValueDbContext
	{
		public KeyValueDbContext(DbContextOptions<KeyValueDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<KeyValue> KeyValues { get; set; } = null!;

	}
}
