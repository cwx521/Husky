using Microsoft.EntityFrameworkCore;

namespace Husky.FileStore.Data
{
	public interface IFileStoreDbContext
	{
		DbContext Normalize();

		DbSet<StoredFile> StoredFiles { get; set; }
	}
}
