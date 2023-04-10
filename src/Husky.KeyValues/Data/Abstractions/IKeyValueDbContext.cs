using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.KeyValues.Data
{
	public interface IKeyValueDbContext : IDbContext, IDisposable, IAsyncDisposable
	{
		DbSet<KeyValue> KeyValues { get; set; }
	}
}
