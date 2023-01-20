using System;

namespace Husky.FileStore
{
	public interface ICloudFileBucket : IFileBucket
	{
		OssProvider Provider { get; }

		Uri SignUri(string fileName, TimeSpan expires);
		void SetAccessControl(string fileName, StoredFileAccessControl accessControl);
	}
}
