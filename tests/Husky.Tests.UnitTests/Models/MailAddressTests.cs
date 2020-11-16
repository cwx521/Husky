using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Mail.Tests
{
	[TestClass()]
	public class MailAddressTests
	{
		[TestMethod()]
		public void ToStringTest() {
			var name = "Weixing";
			var address = "chenwx521@hotmail.com";

			var mailAddress = new MailAddress {
				Address = address
			};
			Assert.AreEqual($"{address}", mailAddress.ToString());

			mailAddress = new MailAddress {
				Name = name,
				Address = address
			};
			Assert.AreEqual($"{name}<{address}>", mailAddress.ToString());
		}

		[TestMethod()]
		public void ParseTest() {
			var name = "Weixing";
			var address = "chenwx521@hotmail.com";
			var str = $"{name}<{address}>";

			var a = MailAddress.Parse(address);
			Assert.IsNull(a.Name);
			Assert.AreEqual(address, a.Address);

			var b = MailAddress.Parse(str);
			Assert.AreEqual(name, b.Name);
			Assert.AreEqual(address, b.Address);
		}

		[TestMethod()]
		public void TryParseTest() {
			var notOk = MailAddress.TryParse("WhatEver<NotMatch>", out _);
			Assert.IsFalse(notOk);

			var name = "Weixing";
			var address = "chenwx521@hotmail.com";
			var str = $"{name}<{address}>";

			var ok = MailAddress.TryParse(address, out var a);
			Assert.IsTrue(ok);
			Assert.IsNull(a.Name);
			Assert.AreEqual(address, a.Address);

			ok = MailAddress.TryParse(str, out var b);
			Assert.IsTrue(ok);
			Assert.AreEqual(name, b.Name);
			Assert.AreEqual(address, b.Address);
		}
	}
}