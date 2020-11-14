using System;
using System.IO;

namespace Husky.FileStore
{
	public interface ICloudFileBucket : IFileBucket
	{
		void OpenBucket(string bucketName);
		Stream Get(string fileName);
		Uri SignUri(string fileName, long expiresInSeconds = 100L * 365 * 24 * 60 * 60);
		StoredFileAt StoredAt { get; }
	}
}
