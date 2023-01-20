using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aliyun.OSS;
using Husky.Principal;

namespace Husky.FileStore.AliyunOss
{
	public class AliyunOssBucket : ICloudFileBucket, IFileBucket
	{
		public AliyunOssBucket(AliyunOssBucketOptions options) {
			_bucketName = options.DefaultBucketName;
			_client = new OssClient(options.Endpoint, options.AccessKeyId, options.AccessKeySecret);
		}

		private string _bucketName;
		private readonly OssClient _client;

		private readonly ObjectMetadata _longTermCacheControlMetadata = new ObjectMetadata {
			CacheControl = $"max-age={_secondsInTenYears}"
		};
		private const int _secondsInTenYears = 10 * 365 * 24 * 60 * 60;

		public OssClient OssClient => _client;
		public OssProvider Provider => OssProvider.Aliyun;

		public void OpenBucket(string bucketName) => _bucketName = bucketName;

		public Stream Get(string fileName) {
			return _client.GetObject(_bucketName, fileName).Content;
		}
		public Uri SignUri(string fileName, TimeSpan expires) {
			return _client.GeneratePresignedUri(_bucketName, fileName, DateTime.Now.Add(expires));
		}
		public void SetAccessControl(string fileName, StoredFileAccessControl accessControl) {
			_client.SetObjectAcl(_bucketName, fileName, (CannedAccessControlList)(int)accessControl);
		}

		public void Put(string fileName, Stream data) {
			_client.PutObject(_bucketName, fileName, data, _longTermCacheControlMetadata);
		}
		public async Task PutAsync(string fileName, Stream data) {
			await Task.Run(() => Put(fileName, data));
		}

		public void Delete(string fileName) {
			_client.DeleteObject(_bucketName, fileName);
		}
		public void Delete(string[] fileNames) {
			_client.DeleteObjects(new DeleteObjectsRequest(_bucketName, fileNames));
		}
	}
}
