using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[Flags]
	enum Abc
	{
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
			var str = Abc.A.ToLower();
			Assert.AreEqual(str, "a");
		}

		[TestMethod()]
		public void ToUpperTest() {
			var str = Abc.A.ToUpper();
			Assert.AreEqual(str, "A");
		}

		[TestMethod()]
		public void ToLabelTest() {
			var a = Abc.A.ToLabel();
			var b = (Abc.B | Abc.C).ToLabel();
			Assert.AreEqual(a, "LabelA");
			Assert.AreEqual(b, "LabelB, LabelC");
		}

		[TestMethod()]
		public void ToLabelWithCssTest() {
			var a = Abc.A.ToLabelWithCss();
			var b = (Abc.B | Abc.C).ToLabelWithCss();
			Assert.AreEqual(a, "<span class='text-body'>LabelA</span>");
			Assert.AreEqual(b, "<span class='text-warning'>LabelB</span>, <span class='text-danger'>LabelC</span>");
		}

		[TestMethod()]
		public void ToDescriptionTest() {
			var a = Abc.A.ToDescription();
			var b = (Abc.B | Abc.C).ToDescription();
			Assert.AreEqual(a, "Description A");
			Assert.AreEqual(b, "Description B, Description C");
		}

		[TestMethod()]
		public void ToDescriptionWithCssTest() {
			var a = Abc.A.ToDescriptionWithCss();
			var b = (Abc.B | Abc.C).ToDescriptionWithCss();
			Assert.AreEqual(a, "<span class='text-body'>Description A</span>");
			Assert.AreEqual(b, "<span class='text-warning'>Description B</span>, <span class='text-danger'>Description C</span>");
		}

		[TestMethod()]
		public void ToSelectListItemsTest() {
			var enumValues = Enum.GetValues(typeof(Abc));
			var firstEnumValue = enumValues.GetValue(0);
			var a = EnumHelper.ToSelectListItems(typeof(Abc));
			Assert.AreEqual(a.Count, enumValues.Length);
			Assert.AreEqual(a.First().Text, ((Enum)firstEnumValue).ToLabel());
			Assert.AreEqual(a.First().Value, firstEnumValue.ToString());
			var b = EnumHelper.ToSelectListItems<Abc>(null, true);
			Assert.AreEqual(b.Count, enumValues.Length);
			Assert.AreEqual(b.First().Text, ((Enum)firstEnumValue).ToLabel());
			Assert.AreEqual(b.First().Value, ((int)firstEnumValue).ToString());
			var c = EnumHelper.ToSelectListItems<Abc>("Foo", true);
			Assert.AreEqual(c.Count, enumValues.Length + 1);
			Assert.AreEqual(c.First().Text, "Foo");
			Assert.IsNull(c.First().Value);
		}
	}
}