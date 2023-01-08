using System;
using System.Collections.Generic;
using System.IO;
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
		public void Delete(string fileName) {
			_client.DeleteObject(_bucketName, fileName);
		}
		public void Delete(string[] fileNames) {
			_client.DeleteObjects(new DeleteObjectsRequest(_bucketName, fileNames));
		}

		public void Tag(string fileName, IDictionary<string, string> tags) {
			var taggingRequest = new SetObjectTaggingRequest(_bucketName, fileName);
			foreach (var i in tags) taggingRequest.AddTag(new Tag { Key = i.Key, Value = i.Value });
			_client.SetObjectTagging(taggingRequest);
		}
		public void Tag(string fileName, string tagKey, string tagValue) {
			Tag(fileName, new Dictionary<string, string> { [tagKey] = tagValue });
		}
		public void TagPrincipalIdentity(string fileName, IPrincipalUser principal) {
			Tag(fileName, new Dictionary<string, string> {
				["UserId"] = principal.Id.ToString(),
				["UserAnonymousId"] = principal.AnonymousId.ToString(),
				["UserName"] = principal.DisplayName
			});
		}
	}
}
