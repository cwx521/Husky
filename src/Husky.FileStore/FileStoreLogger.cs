using System.Linq;
using Husky.FileStore.Data;
using Husky.Principal;

namespace Husky.FileStore
{
	public class FileStoreLogger : IFileStoreLogger
	{
		public FileStoreLogger(IFileStoreDbContext fileStoreDb) {
			_db = fileStoreDb;
		}

		private readonly IFileStoreDbContext _db;

		public void LogFilePut(string fileName, OssProvider storedAt, long contentLength, IPrincipalUser? principal) {
			var row = new StoredFile {
				FileContentLength = contentLength,
				FileName = fileName,
				FileType = StoredFileTypeHelper.Identify(fileName),
				StoredAt = storedAt,
				AnonymousId = principal?.AnonymousId,
				UserId = principal?.Id,
				UserName = principal?.DisplayName,
				AccessControl = StoredFileAccessControl.Default
			};
			_db.StoredFiles.Add(row);
			_db.Normalize().SaveChanges();
		}

		public void LogFileDelete(string fileName) {
			var row = _db.StoredFiles.SingleOrDefault(x => x.FileName == fileName);
			if ( row != null ) {
				row.IsDeleted = true;
				_db.Normalize().SaveChanges();
			}
		}

		public void LogAccessControlChange(string fileName, StoredFileAccessControl accessControl) {
			var row = _db.StoredFiles.SingleOrDefault(x => x.FileName == fileName);
			if ( row != null ) {
				row.AccessControl = accessControl;
				_db.Normalize().SaveChanges();
			}
		}
	}
}
