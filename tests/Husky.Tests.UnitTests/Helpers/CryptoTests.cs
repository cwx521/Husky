using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class CryptoTests
	{
		[TestMethod()]
		public void RandomNumberTest() {
			var rem = 0;
			for ( int i = 0; i < 100; i++ ) {
				var value = Crypto.RandomInt32();
				Assert.AreNotEqual(rem, value);
				rem = value;
			}
		}

		[TestMethod()]
		public void RandomStringTest() {
			var rem = "";
			for ( int i = 0; i < 100; i++ ) {
				var value = Crypto.RandomString();
				Assert.AreNotEqual(rem, value);
				rem = value;
			}
			var a = Crypto.RandomString(8);
			var b = Crypto.RandomString(8);
			var c = Crypto.RandomString(15);
			Assert.AreNotEqual(a, b);
			Assert.AreEqual(8, a.Length);
			Assert.AreEqual(8, b.Length);
			Assert.AreEqual(15, c.Length);
		}

		[TestMethod()]
		public void MD5Test() {
			var str = Crypto.RandomString();
			var a = Crypto.MD5(str);
			var b = str.MD5();
			Assert.AreEqual(a, b);
			Assert.AreEqual(a, b.ToLower());
			Assert.AreEqual(32, a.Length);
		}

		[TestMethod()]
		public void SHA1Test() {
			var str = Crypto.RandomString();
			var a = Crypto.SHA1(str);
			var b = str.SHA1();
			Assert.AreEqual(a, b);
			Assert.AreEqual(a, b.ToLower());
			Assert.AreEqual(40, a.Length);
		}

		[TestMethod()]
		public void SHA256Test() {
			var str = Crypto.RandomString();
			var a = Crypto.SHA256(str);
			var b = str.SHA256();
			Assert.AreEqual(a, b);
			Assert.AreEqual(a, b.ToLower());
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
		public void EncryptDecryptTest() {
			var iv = Crypto.RandomString();
			var key = Crypto.RandomString();
			for ( int length = 0; length < 200; length++ ) {
				var str = Crypto.RandomString(length);
				var encrypted = Crypto.Encrypt(str, iv, key);
				var decrypted = Crypto.Decrypt(encrypted, iv, key);
				Assert.AreNotEqual(str, encrypted);
				Assert.AreEqual(str, decrypted);
			}
		}
	}
}