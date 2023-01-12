using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class StringTestTests
	{
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
			Assert.IsTrue("18812345678".IsCellphone());
			Assert.IsFalse("188123456788".IsCellphone());
			Assert.IsFalse("1881234567".IsCellphone());
			Assert.IsFalse("28812345678".IsCellphone());
		}
	}
}