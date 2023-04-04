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
			var n = 10000;
			var list = new List<string>();
			for ( var i = 0; i < n; i++ ) {
				list.Add(OrderIdGen.New());
				list.Add(OrderIdGen.NewLiteral());
			}
			Assert.AreEqual(n * 2, list.Distinct().Count());
		}

		[TestMethod()]
		public void IsValidTest() {
			var orderNo = OrderIdGen.New();
			var invalidOrderNo = string.Join("", orderNo.Reverse());
			Assert.IsTrue(OrderIdGen.IsValid(orderNo));
			Assert.IsFalse(OrderIdGen.IsValid(invalidOrderNo));
		}

		[TestMethod()]
		public void TryParseTest() {
			var orderNo = OrderIdGen.New();
			var invalidOrderNo = string.Join("", orderNo.Reverse());
			var shouldBeTrue = OrderIdGen.TryParse(orderNo, out var t1);
			var shouldBeFalse = OrderIdGen.TryParse(invalidOrderNo, out _);
			Assert.IsTrue(shouldBeTrue);
			Assert.IsFalse(shouldBeFalse);
			Assert.IsTrue(DateTime.UtcNow.Subtract(t1).TotalSeconds < 5);

			var orderNo2 = OrderIdGen.NewLiteral();
			invalidOrderNo = string.Join("", orderNo2.Reverse());
			shouldBeTrue = OrderIdGen.TryParse(orderNo2, out var t2);
			shouldBeFalse = OrderIdGen.TryParse(invalidOrderNo, out _);
			Assert.IsTrue(shouldBeTrue);
			Assert.IsFalse(shouldBeFalse);
			Assert.IsTrue(DateTime.UtcNow.Subtract(t2).TotalSeconds < 5);
		}
	}
}