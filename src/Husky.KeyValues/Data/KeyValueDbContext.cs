#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.KeyValues.Data
{
	public class KeyValueDbContext : DbContext, IKeyValueDbContext
	{
		public KeyValueDbContext(DbContextOptions<KeyValueDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<KeyValue> KeyValues { get; set; }
	}
}