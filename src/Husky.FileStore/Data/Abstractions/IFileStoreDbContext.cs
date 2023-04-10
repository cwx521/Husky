using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.FileStore.Data
{
	public interface IFileStoreDbContext : IDbContext, IDisposable, IAsyncDisposable
	{
		DbSet<StoredFile> StoredFiles { get; set; }
	}
}
