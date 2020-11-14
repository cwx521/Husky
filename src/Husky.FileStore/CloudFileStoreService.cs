using System;
using System.IO;
using System.Linq;
using Husky.FileStore.Data;
using Husky.Principal;
using Microsoft.EntityFrameworkCore;

namespace Husky.FileStore
{
	public class CloudFileStoreService : ICloudFileStoreService
	{
		public CloudFileStoreService(ICloudFileBucket cloudBucket, IPrincipalUser principal, IFileStoreDbContext fileStoreDb) {
			_bucket = cloudBucket;
			_me = principal;
			_db = fileStoreDb;
		}

		private readonly ICloudFileBucket _bucket;
		private readonly IPrincipalUser _me;
		private readonly IFileStoreDbContext _db;

		public StoredFileAt StoredAt => _bucket.StoredAt;

		public void OpenBucket(string bucketName) => _bucket.OpenBucket(bucketName);

		public Stream Get(string fileName) => _bucket.Get(fileName);

		public Uri SignUri(string fileName, long expiresInSeconds = 3153600000) {
			var row = _db.StoredFiles.IgnoreQueryFilters().SingleOrDefault(x => x.FileName == fileName);
			if ( row == null ) {
				row = new StoredFile();
				EvaluateLiteralPropertyValues(row, fileName, StoredAt, _me);
				_db.StoredFiles.Add(row);
			}
			if ( row.FileUri == null ) {
				row.FileUri = _bucket.SignUri(fileName, expiresInSeconds);
				_db.Normalize().SaveChanges();
			}
			return row.FileUri!;
		}

		public void Put(string fileName, Stream data) {
			_bucket.Put(fileName, data);

			var row = new StoredFile {
				FileUri = _bucket.SignUri(fileName),
				FileContentLength = data.Length
			};
			EvaluateLiteralPropertyValues(row, fileName, StoredAt, _me);

			_db.StoredFiles.Add(row);
			_db.Normalize().SaveChanges();
		}

		public void Delete(string fileName) {
			var row = _db.StoredFiles.SingleOrDefault(x => x.FileName == fileName);
			if ( row != null ) {
				row.IsDeleted = true;
				_db.Normalize().SaveChanges();
			}
			_bucket.Delete(fileName);
		}

		private static void EvaluateLiteralPropertyValues(StoredFile row, string fileName, StoredFileAt storeAt, IPrincipalUser principal) {
			row.FileName = fileName;
			row.FileType = StoredFileTypeHelper.Identify(fileName);
			row.StoredAt = storeAt;
			row.UserId = principal.Id;
			row.AnonymousId = principal.AnonymousId;
			row.UserName = principal.DisplayName;
		}
	}
}
