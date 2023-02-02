namespace Husky.FileStore
{
	public enum StoredFileAccessControl
	{
		[Label("不公开")]
		Private = 0,

		[Label("公开读")]
		PublicRead = 1,

		[Label("公开读和写")]
		PublicReadWrite = 2,

		[Label("继承库设定")]
		Inherit = 3
	}
}
