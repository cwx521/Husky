using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.FileStore.Data;
using Husky.Principal;
using Microsoft.EntityFrameworkCore;

namespace Husky.FileStore
{
	public class FileStoreLogger : IFileStoreLogger
	{
		public FileStoreLogger(IFileStoreDbContext fileStoreDb) {
			_db = fileStoreDb;
		}

		private readonly IFileStoreDbContext _db;

		public async Task LogFilePutAsync(string fileName, OssProvider storedAt, long contentLength, IPrincipalUser? byUser) {
			var record = new StoredFile {
				FileContentLength = contentLength,
				FileName = fileName,
				FileType = StoredFileTypeHelper.Identify(fileName),
				StoredAt = storedAt,
				AnonymousId = byUser?.AnonymousId,
				UserId = byUser?.Id,
				UserName = byUser?.DisplayName,
				AccessControl = StoredFileAccessControl.Inherit,
				CreatedTime = DateTime.Now
			};
			_db.StoredFiles.Add(record);
			await _db.Normalize().SaveChangesAsync();
		}

		public async Task LogFileDeleteAsync(string fileName) {
			await _db.StoredFiles
				.Where(x => x.FileName == fileName)
				.ExecuteUpdateAsync(row =>
					row.SetProperty(x => x.IsDeleted, true)
				);
		}

		public async Task LogAccessControlChangeAsync(string fileName, StoredFileAccessControl accessControl) {
			await _db.StoredFiles
				.Where(x => x.FileName == fileName)
				.ExecuteUpdateAsync(row =>
					row.SetProperty(x => x.AccessControl, accessControl)
				);
		}
	}
}
