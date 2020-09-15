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
			for ( int i = 0; i < n; i++ ) {
				list.Add(OrderIdGen.New());
			}
			Assert.AreEqual(n, list.Distinct().Count());
		}

		[TestMethod()]
		public void IsValidTest() {
			var orderId = OrderIdGen.New();
			var fakeOrderId = string.Join("", orderId.Reverse());
			Assert.IsTrue(OrderIdGen.IsValid(orderId));
			Assert.IsFalse(OrderIdGen.IsValid(fakeOrderId));
		}

		[TestMethod()]
		public void TryParseTest() {
			var orderId = OrderIdGen.New();
			var fakeOrderId = string.Join("", orderId.Reverse());
			var shouldBeTrue = OrderIdGen.TryParse(orderId, out var t1);
			var shouldBeFalse = OrderIdGen.TryParse(fakeOrderId, out _);
			Assert.IsTrue(shouldBeTrue);
			Assert.IsFalse(shouldBeFalse);
			Assert.IsTrue(DateTime.Now.Subtract(t1).TotalSeconds < 1);
		}
	}
}