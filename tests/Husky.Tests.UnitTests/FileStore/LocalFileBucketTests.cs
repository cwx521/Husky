using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.FileStore.LocalStorage.Tests
{
	[TestClass()]
	public class LocalFileBucketTests
	{
		[TestMethod()]
		public void PutTest() {
			var dir = @"E:\Desktop";
			var fileName = "ProducedByUnitTest.jpg";
			var bucket = new LocalFileBucket(dir);
			using var givenData = new FileStream(@"E:\Pictures\Wallpapers\winter.jpg", FileMode.Open, FileAccess.Read);

			bucket.Put(fileName, givenData);
			Assert.IsTrue(File.Exists($"{dir}\\{fileName}"));

			using var readStream = bucket.Get(fileName);
			Assert.AreEqual(givenData.Length, readStream.Length);
			readStream.Dispose();

			bucket.Delete(fileName);
			Assert.IsFalse(File.Exists($"{dir}\\{fileName}"));
		}
	}
}