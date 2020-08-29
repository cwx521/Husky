using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Tests
{
	[Flags]
	enum ABC {
		[Label("LabelA", Description = "Description A", CssClass = "text-body")]
		A,
		[Label("LabelB", Description = "Description B", CssClass = "text-warning")]
		B,
		[Label("LabelC", Description = "Description C", CssClass = "text-danger")]
		C
	}

	[TestClass()]
	public class EnumHelperTests
	{
		[TestMethod()]
		public void ToLowerTest() {
			var str = ABC.A.ToLower();
			Assert.AreEqual(str, "a");
		}

		[TestMethod()]
		public void ToUpperTest() {
			var str = ABC.A.ToUpper();
			Assert.AreEqual(str, "A");
		}

		[TestMethod()]
		public void ToLabelTest() {
			var a = ABC.A.ToLabel();
			var b = (ABC.B | ABC.C).ToLabel();
			Assert.AreEqual(a, "LabelA");
			Assert.AreEqual(b, "LabelB, LabelC");
		}

		[TestMethod()]
		public void ToLabelWithCssTest() {
			var a = ABC.A.ToLabelWithCss();
			var b = (ABC.B | ABC.C).ToLabelWithCss();
			Assert.AreEqual(a, "<span class='text-body'>LabelA</span>");
			Assert.AreEqual(b, "<span class='text-warning'>LabelB</span>, <span class='text-danger'>LabelC</span>");
		}

		[TestMethod()]
		public void ToDescriptionTest() {
			var a = ABC.A.ToDescription();
			var b = (ABC.B | ABC.C).ToDescription();
			Assert.AreEqual(a, "Description A");
			Assert.AreEqual(b, "Description B, Description C");
		}

		[TestMethod()]
		public void ToDescriptionWithCssTest() {
			var a = ABC.A.ToDescriptionWithCss();
			var b = (ABC.B | ABC.C).ToDescriptionWithCss();
			Assert.AreEqual(a, "<span class='text-body'>Description A</span>");
			Assert.AreEqual(b, "<span class='text-warning'>Description B</span>, <span class='text-danger'>Description C</span>");
		}
	}
}