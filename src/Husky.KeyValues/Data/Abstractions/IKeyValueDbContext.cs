using Microsoft.EntityFrameworkCore;

namespace Husky.KeyValues.Data
{
	public interface IKeyValueDbContext
	{
		DbContext Normalize();

		DbSet<KeyValue> KeyValues { get; set; }
	}
}
