using Husky.Principal;

namespace Husky.FileStore
{
	public interface IFileStoreLogger
	{
		void LogFilePut(string fileName, OssProvider storedAt, long contentLength, IPrincipalUser? byUser);
		void LogFileDelete(string fileName);
		void LogAccessControlChange(string fileName, StoredFileAccessControl accessControl);
	}
}
