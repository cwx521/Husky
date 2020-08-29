using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Tests
{
	[TestClass()]
	public class FormatHelperTests
	{
		[TestMethod()]
		public void TrimEndTest() {
			Assert.AreEqual(FormatHelper.TrimEnd(12.00m), "12");
			Assert.AreEqual(FormatHelper.TrimEnd(12.345m), "12.345");
			Assert.AreEqual(12.30m.TrimEnd(), "12.3");
			Assert.AreEqual(12.03m.TrimEnd(), "12.03");
			Assert.AreEqual(12.030m.TrimEnd(), "12.03");
		}
	}
}