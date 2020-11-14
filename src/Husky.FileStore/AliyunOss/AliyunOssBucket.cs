using System;
using System.IO;
using Aliyun.OSS;

namespace Husky.FileStore.AliyunOss
{
	public class AliyunOssBucket : ICloudFileBucket
	{
		public AliyunOssBucket(AliyunOssBucketOptions options) {
			_bucketName = options.DefaultBucketName;
			_client = new OssClient(options.Endpoint, options.AccessKeyId, options.AccessKeySecret);
		}

		private string _bucketName;
		private readonly OssClient _client;

		public OssClient OriginalClient => _client;
		public StoredFileAt StoredAt => StoredFileAt.Aliyun;

		public void OpenBucket(string bucketName) => _bucketName = bucketName;

		public Stream Get(string fileName) => _client.GetObject(_bucketName, fileName).Content;
		public Uri SignUri(string fileName, long expiresInSeconds = 100L * 365 * 24 * 60 * 60) => _client.GeneratePresignedUri(_bucketName, fileName, DateTime.Now.AddSeconds(expiresInSeconds));
		public void Put(string fileName, Stream data) => _client.PutObject(_bucketName, fileName, data);
		public void Delete(string fileName) => _client.DeleteObject(_bucketName, fileName);
	}
}
