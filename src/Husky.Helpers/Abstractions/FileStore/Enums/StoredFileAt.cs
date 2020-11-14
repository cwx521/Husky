namespace Husky.FileStore
{
	public enum StoredFileAt
	{
		[Label("本地磁盘")]
		LocalDisk,

		[Label("阿里云")]
		Aliyun,

		[Label("腾讯云")]
		TencentCloud,

		[Label("华为云")]
		HuaweiCloud,

		[Label("AWS")]
		Aws,

		[Label("Azure")]
		Azure
	}
}
