using System.IO;
using System.Threading.Tasks;

namespace Husky.FileStore
{
	public interface IFileBucket
	{
		void OpenBucket(string bucketName);
		Stream Get(string fileName);
		void Put(string fileName, Stream data);
		Task PutAsync(string fileName, Stream data);
		void Delete(string fileName);
		void Delete(string[] fileNames);
	}
}
