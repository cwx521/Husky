using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Mail.Tests
{
	[TestClass()]
	public class MailAddressTests
	{
		[TestMethod()]
		public void ParseTest() {
			var name = "Weixing";
			var address = "chenwx521@hotmail.com";
			var str = $"{name}<{address}>";
			var a = MailAddress.Parse(address);
			Assert.IsNull(a.Name);
			Assert.AreEqual(a.Address, address);
			var b = MailAddress.Parse(str);
			Assert.AreEqual(b.Name, name);
			Assert.AreEqual(b.Address, address);
		}

		[TestMethod()]
		public void TryParseTest() {
			var notOk = MailAddress.TryParse("WhatEver<NotMatch>", out _);
			Assert.IsFalse(notOk);

			var name = "Weixing";
			var address = "chenwx521@hotmail.com";
			var str = $"{name}<{address}>";
			var ok = MailAddress.TryParse(str, out var mailAddress);
			Assert.IsTrue(ok);
			Assert.AreEqual(mailAddress.Name, name);
			Assert.AreEqual(mailAddress.Address, address);
		}
	}
}