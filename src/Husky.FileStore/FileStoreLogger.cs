using System;
using System.Collections.Generic;
using System.Linq;
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

		public void LogFilePut(string fileName, OssProvider storedAt, long contentLength, IPrincipalUser? byUser, IDictionary<string, string>? tags) {
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
			_db.Normalize().SaveChanges();
		}

		public void LogFileDelete(string fileName) {
			_db.StoredFiles
				.Where(x => x.FileName == fileName)
				.ExecuteUpdate(row =>
					row.SetProperty(x => x.IsDeleted, true)
				);
		}

		public void LogAccessControlChange(string fileName, StoredFileAccessControl accessControl) {
			_db.StoredFiles
				.Where(x => x.FileName == fileName)
				.ExecuteUpdate(row =>
					row.SetProperty(x => x.AccessControl, accessControl)
				);
		}
	}
}
