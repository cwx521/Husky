using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.FileStore.AliyunOss.Tests
{
	[TestClass()]
	public class AliyunOssBucketTests
	{
		public AliyunOssBucketTests() {
			var config = new ConfigurationManager();
			config.AddUserSecrets(GetType().Assembly);
			_options = config.GetSection("AliyunOssBucket").Get<AliyunOssBucketOptions>();
		}

		private readonly AliyunOssBucketOptions _options;

		[TestMethod()]
		public void MashalTest() {
			var oss = new AliyunOssBucket(_options);
			oss.OpenBucket(_options.DefaultBucketName);

			var fileName = $"UnitTest/{Guid.NewGuid()}.jpg";
			var bytes = Crypto.RandomBytes();
			using var writeStream = new MemoryStream(bytes);
			oss.Put(fileName, writeStream);
			writeStream.Dispose();

			using var readStream = oss.Get(fileName);
			Assert.AreEqual(bytes.Length, readStream.Length);
			readStream.Dispose();

			var uri = oss.SignUri(fileName, TimeSpan.FromMinutes(1));
			Assert.IsNotNull(uri);

			oss.SetAccessControl(fileName, StoredFileAccessControl.PublicRead);
			oss.Delete(fileName);
		}
	}
}