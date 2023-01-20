using System.Collections.Generic;
using System.Threading.Tasks;
using Husky.Principal;

namespace Husky.FileStore
{
	public interface IFileStoreLogger
	{
		Task LogFilePutAsync(string fileName, OssProvider storedAt, long contentLength, IPrincipalUser? byUser, IDictionary<string, string> tags);
		Task LogFileDeleteAsync(string fileName);
		Task LogAccessControlChangeAsync(string fileName, StoredFileAccessControl accessControl);
	}
}
