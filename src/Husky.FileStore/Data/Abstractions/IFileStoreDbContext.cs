using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.FileStore.Data
{
	public interface IFileStoreDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		DbSet<StoredFile> StoredFiles { get; set; }
		DbSet<StoredFileTag> StoredFileTags { get; set; }
	}
}
