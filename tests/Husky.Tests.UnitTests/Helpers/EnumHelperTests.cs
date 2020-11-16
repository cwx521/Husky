using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[Flags]
	internal enum Abc
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
			Assert.AreEqual("a", str);
		}

		[TestMethod()]
		public void ToUpperTest() {
			var str = Abc.A.ToUpper();
			Assert.AreEqual("A", str);
		}

		[TestMethod()]
		public void ToLabelTest() {
			var a = Abc.A.ToLabel();
			var b = (Abc.B | Abc.C).ToLabel();
			Assert.AreEqual("LabelA", a);
			Assert.AreEqual("LabelB, LabelC", b);
		}

		[TestMethod()]
		public void ToLabelWithCssTest() {
			var a = Abc.A.ToLabelWithCss();
			var b = (Abc.B | Abc.C).ToLabelWithCss();
			Assert.AreEqual("<span class='text-body'>LabelA</span>", a);
			Assert.AreEqual("<span class='text-warning'>LabelB</span>, <span class='text-danger'>LabelC</span>", b);
		}

		[TestMethod()]
		public void ToDescriptionTest() {
			var a = Abc.A.ToDescription();
			var b = (Abc.B | Abc.C).ToDescription();
			Assert.AreEqual("Description A", a);
			Assert.AreEqual("Description B, Description C", b);
		}

		[TestMethod()]
		public void ToDescriptionWithCssTest() {
			var a = Abc.A.ToDescriptionWithCss();
			var b = (Abc.B | Abc.C).ToDescriptionWithCss();
			Assert.AreEqual("<span class='text-body'>Description A</span>", a);
			Assert.AreEqual("<span class='text-warning'>Description B</span>, <span class='text-danger'>Description C</span>", b);
		}

		[TestMethod()]
		public void ToSelectListItemsTest() {
			var enumValues = Enum.GetValues(typeof(Abc));
			var firstEnumValue = enumValues.GetValue(0);

			var a = EnumHelper.ToSelectListItems(typeof(Abc));
			Assert.AreEqual(enumValues.Length, a.Count);
			Assert.AreEqual(((Enum)firstEnumValue).ToLabel(), a.First().Text);
			Assert.AreEqual(firstEnumValue.ToString(), a.First().Value);

			var b = EnumHelper.ToSelectListItems<Abc>(null, true);
			Assert.AreEqual(enumValues.Length, b.Count);
			Assert.AreEqual(((Enum)firstEnumValue).ToLabel(), b.First().Text);
			Assert.AreEqual(((int)firstEnumValue).ToString(), b.First().Value);

			var c = EnumHelper.ToSelectListItems<Abc>("Foo", true);
			Assert.AreEqual(enumValues.Length + 1, c.Count);
			Assert.AreEqual("Foo", c.First().Text);
			Assert.IsNull(c.First().Value);
		}
	}
}