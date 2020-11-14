using System.IO;

namespace Husky.FileStore
{
	public interface IFileBucket
	{
		void Put(string fileName, Stream data);
		void Delete(string fileName);
	}
}
