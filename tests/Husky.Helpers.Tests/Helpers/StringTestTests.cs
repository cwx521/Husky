using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Tests
{
	[TestClass()]
	public class StringTestTests
	{
		[TestMethod()]
		public void GetSexFromMainlandSocialNumberTest() {
			var id = "320501198305216035";
			var isOk = id.IsMainlandSocialNumber();
			var sex = id.GetSexFromMainlandSocialNumber();
			Assert.IsTrue(isOk);
			Assert.AreEqual(sex, Sex.Male);

			id = "32041119820731312X";
			isOk = id.IsMainlandSocialNumber();
			sex = id.GetSexFromMainlandSocialNumber();
			Assert.IsTrue(isOk);
			Assert.AreEqual(sex, Sex.Female);

			id = "320411198207123456";
			isOk = id.IsMainlandSocialNumber();
			sex = id.GetSexFromMainlandSocialNumber();
			Assert.IsFalse(isOk);
			Assert.IsNull(sex);
		}

		[TestMethod()]
		public void IsUrlTest() {
			Assert.IsTrue("http://www.xingyisoftware.com".IsUrl());
			Assert.IsTrue("https://www.xingyisoftware.com".IsUrl());
			Assert.IsTrue("protocol://www.xingyisoftware.com".IsUrl());
			Assert.IsFalse("www.xingyisoftware.com".IsUrl());
		}

		[TestMethod()]
		public void IsEmailTest() {
			Assert.IsTrue("chenwx521@hotmail.com".IsEmail());
			Assert.IsTrue("chenwx521@xxxx.com.cn".IsEmail());
			Assert.IsTrue("chen.wx@xxxx.com.cn".IsEmail());
			Assert.IsFalse("chenwx521@xxxx".IsEmail());
		}

		[TestMethod()]
		public void IsMainlandMobileTest() {
			Assert.IsTrue("18812345678".IsMainlandMobile());
			Assert.IsFalse("188123456788".IsMainlandMobile());
			Assert.IsFalse("1881234567".IsMainlandMobile());
			Assert.IsFalse("28812345678".IsMainlandMobile());
		}
	}
}