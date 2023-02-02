using System.IO;

namespace Husky.FileStore
{
	public enum StoredFileType
	{
		[Label("图片")]
		Image,

		[Label("视频")]
		Video,

		[Label("文档")]
		Document,

		[Label("压缩包")]
		Zip,

		[Label("其它")]
		Else = 99
	}

	public static class StoredFileTypeHelper
	{
		public static StoredFileType Identify(string fileName) => Path.GetExtension(fileName).ToLower() switch {
			".png" or ".gif" or ".jpg" or ".jpeg" => StoredFileType.Image,
			".mp4" or ".avi" or ".mpeg" => StoredFileType.Video,
			".doc" or ".docx" or ".xls" or ".xlsx" or ".ppt" or ".pptx" or ".txt" => StoredFileType.Document,
			".zip" or ".rar" or ".7z" or ".gz" or ".iso" => StoredFileType.Zip,
			_ => StoredFileType.Else,
		};
	}
}
