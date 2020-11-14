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
			if ( string.IsNullOrEmpty(options.AccessKeyId) || string.IsNullOrEmpty(options.AccessKeySecret) ) {
				return null;
			}
			return new AliyunOssBucket(options);
		}

		[TestMethod()]
		public void MashalTest() {
			var key = "UnitTest/" + Guid.NewGuid().ToString() + ".png";
			var bytes = Crypto.RandomBytes();
			using var stream = new MemoryStream(bytes);

			var oss = BuildAliyunOssService();
			oss?.Put(key, stream);
			var uri = oss?.SignUri(key);
			var read = oss?.Get(key);
			oss?.Delete(key);
		}
	}
}