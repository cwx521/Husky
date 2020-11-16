using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class OrderIdGenTests
	{
		[TestMethod()]
		public void NewTest() {
			var n = 2000;
			var list = new List<string>();
			for ( var i = 0; i < n; i++ ) {
				list.Add(OrderIdGen.New());
			}
			Assert.AreEqual(n, list.Distinct().Count());
		}

		[TestMethod()]
		public void IsValidTest() {
			var orderNo = OrderIdGen.New();
			var fakeOrderNo = string.Join("", orderNo.Reverse());
			Assert.IsTrue(OrderIdGen.IsValid(orderNo));
			Assert.IsFalse(OrderIdGen.IsValid(fakeOrderNo));
		}

		[TestMethod()]
		public void TryParseTest() {
			var orderNo = OrderIdGen.New();
			var fakeOrderNo = string.Join("", orderNo.Reverse());
			var shouldBeTrue = OrderIdGen.TryParse(orderNo, out var t1);
			var shouldBeFalse = OrderIdGen.TryParse(fakeOrderNo, out _);
			Assert.IsTrue(shouldBeTrue);
			Assert.IsFalse(shouldBeFalse);
			Assert.IsTrue(DateTime.Now.Subtract(t1).TotalSeconds < 2);
		}
	}
}