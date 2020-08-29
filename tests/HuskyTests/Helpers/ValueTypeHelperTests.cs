using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky.Syntactic.Natural;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Syntactic.Natural.Tests
{
	[TestClass()]
	public class ValueTypeHelperTests
	{
		[TestMethod()]
		public void IsTest() {
			Assert.IsTrue(1.Is(1, 2, 3));
			Assert.IsTrue("a".Is("1", "2", "a"));
			Assert.IsTrue("a".Is("a"));
			Assert.IsFalse(1.Is(2, 3));
			Assert.IsFalse("a".Is("b"));
		}

		[TestMethod()]
		public void IsNotTest() {
			Assert.IsFalse(1.IsNot(1, 2, 3));
			Assert.IsFalse("a".IsNot("1", "2", "a"));
			Assert.IsFalse("a".IsNot("a"));
			Assert.IsTrue(1.IsNot(2, 3));
			Assert.IsTrue("a".IsNot("b"));
		}
	}
}