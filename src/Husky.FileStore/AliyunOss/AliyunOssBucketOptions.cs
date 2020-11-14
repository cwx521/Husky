namespace Husky.FileStore.AliyunOss
{
	public class AliyunOssBucketOptions
	{
		public string AccessKeyId { get; set; } = null!;
		public string AccessKeySecret { get; set; } = null!;
		public string Endpoint { get; set; } = null!;
		public string DefaultBucketName { get; set; } = null!;
	}
}
