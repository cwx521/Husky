using Microsoft.EntityFrameworkCore;

namespace Husky.FileStore.Data
{
	public static class FileStoreDbModelBuilderHelper
	{
		public static void OnFileStoreDbModelCreating(this ModelBuilder mb) {
			mb.Entity<StoredFile>(storedFile => {
				storedFile.HasQueryFilter(x => !x.IsDeleted);
			});
		}
	}
}
