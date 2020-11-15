using System;
using System.Collections.Generic;
using System.IO;
using Husky.Principal;

namespace Husky.FileStore
{
	public interface ICloudFileBucket : IFileBucket
	{
		OssProvider Provider { get; }

		Uri SignUri(string fileName, TimeSpan expires);
		void SetAccessControl(string fileName, StoredFileAccessControl accessControl);

		void Tag(string fileName, string tagKey, string tagValue);
		void Tag(string fileName, IDictionary<string, string> tags);
		void TagPrincipalIdentity(string fileName, IPrincipalUser principal);
	}
}
