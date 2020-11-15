using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.FileStore.AliyunOss.Tests
{
	[TestClass()]
	public class AliyunOssBucketTests
	{
		private static AliyunOssBucket BuildAliyunOssService() {
			var options = new AliyunOssBucketOptions {
				AccessKeyId = "",
				AccessKeySecret = "",
				DefaultBucketName = "",
				Endpoint = "oss-cn-hangzhou.aliyuncs.com",
			};
			return string.IsNullOrEmpty(options.AccessKeyId) || string.IsNullOrEmpty(options.AccessKeySecret)
				? null
				: new AliyunOssBucket(options);
		}

		[TestMethod()]
		public void MashalTest() {
			var oss = BuildAliyunOssService();
			if ( oss != null ) {
				var fileName = $"UnitTest/{Guid.NewGuid()}.jpg";

				var bytes = Crypto.RandomBytes();
				using var stream = new MemoryStream(bytes);
				oss.Put(fileName, stream);

				var uri = oss.SignUri(fileName, TimeSpan.FromMinutes(1));
				var read = oss.Get(fileName);
				oss.Delete(fileName);
			}
		}
	}
}