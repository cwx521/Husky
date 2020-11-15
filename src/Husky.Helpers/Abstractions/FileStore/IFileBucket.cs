using System.IO;

namespace Husky.FileStore
{
	public interface IFileBucket
	{
		void OpenBucket(string bucketName);
		Stream Get(string fileName);
		void Put(string fileName, Stream data);
		void Delete(string fileName);
		void Delete(string[] fileNames);
	}
}
