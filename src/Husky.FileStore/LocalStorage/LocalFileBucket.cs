using System;
using System.IO;
using System.Threading.Tasks;

namespace Husky.FileStore.LocalStorage
{
	public class LocalFileBucket : IFileBucket
	{
		public LocalFileBucket(string rootPath) {
			ValidateRootPath(rootPath);
			_rootPath = rootPath;
		}

		private string _rootPath;

		public void OpenBucket(string bucketName) {
			ValidateRootPath(bucketName);
			_rootPath = bucketName;
		}

		public Stream Get(string fileName) {
			return new FileStream(GetPhysicalPath(_rootPath, fileName), FileMode.Open);
		}

		public void Put(string fileName, Stream data) {
			PutAsync(fileName, data).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public async Task PutAsync(string fileName, Stream data) {
			var type = StoredFileTypeHelper.Identify(fileName);
			if (type == StoredFileType.Else) {
				throw new InvalidDataException($"Not allowed to upload {Path.GetExtension(fileName)} files or files without any extension name.");
			}

			var filePath = GetPhysicalPath(_rootPath, fileName);
			var directory = Path.GetDirectoryName(filePath)!;

			Directory.CreateDirectory(directory);
			using var fileStream = new FileStream(filePath, FileMode.Create);

			var bufferSize = 4096;
			var buffer = new byte[bufferSize];
			var i = 0;
			while ((i = data.Read(buffer, 0, bufferSize)) != 0) {
				await fileStream.WriteAsync(buffer.AsMemory(0, i));
				await fileStream.FlushAsync();
			}
		}

		public void Delete(string fileName) {
			try {
				File.Delete(GetPhysicalPath(_rootPath, fileName));
			}
			catch (DirectoryNotFoundException) { }
			catch { throw; }
		}

		public void Delete(string[] fileNames) {
			foreach (var fileName in fileNames) {
				Delete(fileName);
			}
		}

		private static string GetPhysicalPath(string rootPath, string fileName) {
			return Path.Combine(rootPath, fileName.TrimStart('/', '\\'));
		}

		private static void ValidateRootPath(string rootPath) {
			if (!Path.IsPathRooted(rootPath)) {
				throw new ArgumentException($"{rootPath} is not a root path");
			}
		}
	}
}
