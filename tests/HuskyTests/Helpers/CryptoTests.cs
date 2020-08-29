using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class CryptoTests
	{
		[TestMethod()]
		public void RandomNumberTest() {
			var a = Crypto.RandomNumber();
			var b = Crypto.RandomNumber();
			Assert.AreNotEqual(a, b);
		}

		[TestMethod()]
		public void RandomStringTest() {
			var a = Crypto.RandomString(8);
			var b = Crypto.RandomString(8);
			var c = Crypto.RandomString(16);
			Assert.AreNotEqual(a, b);
			Assert.AreEqual(8, a.Length);
			Assert.AreEqual(8, b.Length);
			Assert.AreEqual(16, c.Length);
		}

		[TestMethod()]
		public void MD5Test() {
			var str = Crypto.RandomString();
			var a = Crypto.MD5(str);
			var b = str.MD5();
			Assert.AreEqual(a, b);
			Assert.AreEqual(32, a.Length);
		}

		[TestMethod()]
		public void SHA1Test() {
			var str = Crypto.RandomString();
			var a = Crypto.SHA1(str);
			var b = str.SHA1();
			Assert.AreEqual(a, b);
			Assert.AreEqual(40, a.Length);
		}

		[TestMethod()]
		public void SHA256Test() {
			var str = Crypto.RandomString();
			var a = Crypto.SHA256(str);
			var b = str.SHA256();
			Assert.AreEqual(a, b);
			Assert.AreEqual(64, a.Length);
		}

		[TestMethod()]
		public void HmacMD5Test() {
			var str = Crypto.RandomString();
			var key = Crypto.RandomString();
			var a = Crypto.HmacMD5(str, key);
			var b = str.HmacMD5(key);
			var c = str.MD5();
			Assert.AreEqual(a, b);
			Assert.AreNotEqual(a, c);
			Assert.AreEqual(32, a.Length);
		}

		[TestMethod()]
		public void HmacSHA1Test() {
			var str = Crypto.RandomString();
			var key = Crypto.RandomString();
			var a = Crypto.HmacSHA1(str, key);
			var b = str.HmacSHA1(key);
			var c = str.SHA1();
			Assert.AreEqual(a, b);
			Assert.AreNotEqual(a, c);
			Assert.AreEqual(40, a.Length);
		}

		[TestMethod()]
		public void HmacSHA256Test() {
			var str = Crypto.RandomString();
			var key = Crypto.RandomString();
			var a = Crypto.HmacSHA256(str, key);
			var b = str.HmacSHA256(key);
			var c = str.SHA256();
			Assert.AreEqual(a, b);
			Assert.AreNotEqual(a, c);
			Assert.AreEqual(64, a.Length);
		}

		[TestMethod()]
		public void EncryptTest() {
			var str = Crypto.RandomString(200);
			var key = Crypto.RandomString();
			var encrypted = Crypto.Encrypt(str, key);
			Assert.AreNotEqual(str, encrypted);
		}

		[TestMethod()]
		public void DecryptTest() {
			var str = Crypto.RandomString(200);
			var key = Crypto.RandomString();
			var encrypted = Crypto.Encrypt(str, key);
			var decrypted = Crypto.Decrypt(encrypted, key);
			Assert.AreNotEqual(str, encrypted);
			Assert.AreEqual(str, decrypted);
		}
	}
}