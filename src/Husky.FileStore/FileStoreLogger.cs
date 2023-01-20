using System;
using System.Collections.Generic;
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

		public async Task LogFilePutAsync(string fileName, OssProvider storedAt, long contentLength, IPrincipalUser? byUser, IDictionary<string, string>? tags) {
			var record = new StoredFile {
				FileContentLength = contentLength,
				FileName = fileName,
				FileType = StoredFileTypeHelper.Identify(fileName),
				StoredAt = storedAt,
				AnonymousId = byUser?.AnonymousId,
				UserId = byUser?.Id,
				UserName = byUser?.DisplayName,
				AccessControl = StoredFileAccessControl.Default,
				CreatedTime = DateTime.Now
			};
			if (tags != null) {
				record.Tags.AddRange(
					tags.Select(x => new StoredFileTag {
						Key = x.Key,
						Value = x.Value
					})
				);
			}
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
