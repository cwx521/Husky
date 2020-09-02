using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky.Principal.Implements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Principal.Implements.Tests
{
	[TestClass()]
	public class IdentityEncryptorTests
	{
		[TestMethod()]
		public void EncryptTest() {
			var token = Crypto.RandomString();
			var identity = new Identity { DisplayName = "Weixing", IdString = "123" };
			var encryptor = new IdentityEncryptor();
			var encrypted = encryptor.Encrypt(identity, token);
			var decrypted = encryptor.Decrypt(encrypted, token);

			Assert.AreEqual(identity.DisplayName, decrypted.DisplayName);
			Assert.AreEqual(identity.IdString, decrypted.IdString);

			var nullResult = encryptor.Decrypt(encrypted, "invalid token");
			Assert.IsNull(nullResult);
		}
	}
}