using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.KeyValues.Data
{
	public interface IKeyValueDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		DbSet<KeyValue> KeyValues { get; set; }
	}
}
