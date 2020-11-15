using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Husky.Principal;

namespace Husky.FileStore.LocalStorage
{
	public class LocalFileBucket : IFileBucket
	{
		public LocalFileBucket(string rootPath) {
			ValidateRootPath(_rootPath = rootPath);
		}

		private string _rootPath;

		public void OpenBucket(string bucketName) => ValidateRootPath(_rootPath = bucketName);

		public Stream Get(string fileName) {
			return new FileStream(GetPhysicalPath(fileName), FileMode.Open);
		}

		public void Put(string fileName, Stream data) {
			var type = StoredFileTypeHelper.Identify(fileName);
			if ( type == StoredFileType.Else ) {
				throw new InvalidDataException($"Not allowed to upload {Path.GetExtension(fileName)} files or files without any extension name.");
			}

			var filePath = GetPhysicalPath(fileName);
			var directory = Path.GetDirectoryName(filePath)!;
			var bytes = new byte[data.Length];

			data.Read(bytes);
			Directory.CreateDirectory(directory);
			File.WriteAllBytes(filePath, bytes);
		}

		public void Delete(string fileName) {
			try {
				File.Delete(GetPhysicalPath(fileName));
			}
			catch ( DirectoryNotFoundException ) { }
			catch { throw; }
		}

		public void Delete(string[] fileNames) {
			foreach ( var fileName in fileNames ) {
				Delete(fileName);
			}
		}

		private string GetPhysicalPath(string fileName) => Path.Combine(_rootPath, fileName.TrimStart('/', '\\'));

		private static void ValidateRootPath(string rootPath) {
			if ( !Path.IsPathRooted(rootPath) ) {
				throw new InvalidProgramException($"{rootPath} is not a root path");
			}
		}
	}
}
