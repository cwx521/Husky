#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

using Microsoft.EntityFrameworkCore;

namespace Husky.FileStore.Data
{
	public class FileStoreDbContext : DbContext, IFileStoreDbContext
	{
		public FileStoreDbContext(DbContextOptions<FileStoreDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<StoredFile> StoredFiles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyAdditionalCustomizedAnnotations();
		}
	}
}
