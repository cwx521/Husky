using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class StringCastTests
	{
		[TestMethod()]
		public void AsIntTest() {
			Assert.AreEqual(123, StringCast.AsInt("123"));
			Assert.AreEqual(123, StringCast.AsInt("abc", 123));
		}

		[TestMethod()]
		public void AsBoolTest() {
			Assert.AreEqual(true, StringCast.AsBool("True"));
			Assert.AreEqual(true, StringCast.AsBool("tRuE"));
			Assert.AreEqual(false, StringCast.AsBool("falSe"));
			Assert.AreEqual(true, StringCast.AsBool("true"));
			Assert.AreEqual(false, StringCast.AsBool("false"));
			Assert.AreEqual(true, StringCast.AsBool("abc", true));
			Assert.AreEqual(false, StringCast.AsBool("abc", false));
			Assert.AreEqual(true, StringCast.AsBool("true", false));
		}

		[TestMethod()]
		public void AsGuidTest() {
			var guid = Guid.NewGuid();
			var str = guid.ToString();
			Assert.AreEqual(guid, StringCast.AsGuid(str));
			Assert.AreEqual(guid, StringCast.AsGuid("abc", guid));
		}

		[TestMethod()]
		public void AsTest() {
			Assert.AreEqual(-1, StringCast.As<int>("-1"));
			Assert.AreEqual(2343546766787981, StringCast.As<long>("2343546766787981"));
			Assert.AreEqual(true, StringCast.As<bool>("true"));

			Assert.AreEqual(1, StringCast.As("abc", 1));
			Assert.AreEqual(true, StringCast.As("abc", true));

			var dt = DateTime.Now;
			Assert.AreEqual(dt.ToString(), StringCast.As<DateTime>(dt.ToString()).ToString());
			var g = Guid.NewGuid();
			Assert.AreEqual(g, StringCast.As<Guid>(g.ToString()));
		}

		[TestMethod()]
		public void HexToIntTest() {
			Assert.AreEqual(14, StringCast.HexToInt("e"));
			Assert.AreEqual(15, StringCast.HexToInt("F"));
			Assert.AreEqual(15, StringCast.HexToInt("0xF"));
			Assert.AreEqual(2 * 16 + 15, StringCast.HexToInt("2F"));
			Assert.AreEqual(2 * 16 + 15, StringCast.HexToInt("0x2F"));
		}

		[TestMethod()]
		public void MaskTest() {
			Assert.AreEqual("a**", StringCast.Mask("abc"));
			Assert.AreEqual("188****8888", StringCast.Mask("18888888888"));
			Assert.AreEqual("c********@hotmail.com", StringCast.Mask("chenwx521@hotmail.com"));
		}

		[TestMethod()]
		public void BetterDisplayCardNumberTest() {
			Assert.AreEqual("1234 1234 1234 1234", StringCast.BetterDisplayCardNumber("1234123412341234"));
			Assert.AreEqual("1234 1234 1234 1234 123", StringCast.BetterDisplayCardNumber("1234123412341234123"));
		}
	}
}