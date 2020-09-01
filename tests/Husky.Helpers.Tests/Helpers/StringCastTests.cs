using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Tests
{
	[TestClass()]
	public class StringCastTests
	{
		[TestMethod()]
		public void NullAsEmptyTest() {
			Assert.AreEqual(StringCast.NullAsEmpty(null), "");
		}

		[TestMethod()]
		public void EmptyAsNullTest() {
			Assert.AreEqual(StringCast.EmptyAsNull(""), null);
		}

		[TestMethod()]
		public void AsIntTest() {
			Assert.AreEqual(StringCast.AsInt("123"), 123);
			Assert.AreEqual(StringCast.AsInt("abc", 123), 123);
		}

		[TestMethod()]
		public void AsBoolTest() {
			Assert.AreEqual(StringCast.AsBool("True"), true);
			Assert.AreEqual(StringCast.AsBool("tRuE"), true);
			Assert.AreEqual(StringCast.AsBool("falSe"), false);
			Assert.AreEqual(StringCast.AsBool("true"), true);
			Assert.AreEqual(StringCast.AsBool("false"), false);
			Assert.AreEqual(StringCast.AsBool("abc", true), true);
			Assert.AreEqual(StringCast.AsBool("abc", false), false);
			Assert.AreEqual(StringCast.AsBool("true", false), true);
		}

		[TestMethod()]
		public void AsGuidTest() {
			var guid = Guid.NewGuid();
			var str = guid.ToString();
			Assert.AreEqual(StringCast.AsGuid(str), guid);
			Assert.AreEqual(StringCast.AsGuid("abc", guid), guid);
		}

		[TestMethod()]
		public void AsTimeSpanTest() {
			var timespan = TimeSpan.FromMinutes(25);
			var str = timespan.ToString();
			Assert.AreEqual(StringCast.AsTimeSpan(str), timespan);
			Assert.AreEqual(StringCast.AsTimeSpan("abc", timespan), timespan);
		}

		[TestMethod()]
		public void AsTest() {
			Assert.AreEqual(StringCast.As<int>("-1"), -1);
			Assert.AreEqual(StringCast.As<long>("2343546766787981"), 2343546766787981);
			Assert.AreEqual(StringCast.As<bool>("true"), true);

			Assert.AreEqual(StringCast.As("abc", 1), 1);
			Assert.AreEqual(StringCast.As("abc", true), true);

			var dt = DateTime.Now;
			Assert.AreEqual(StringCast.As<DateTime>(dt.ToString()).ToString(), dt.ToString());
			var ts = TimeSpan.FromSeconds(120);
			Assert.AreEqual(StringCast.As<TimeSpan>(ts.ToString()), ts);
			var g = Guid.NewGuid();
			Assert.AreEqual(StringCast.As<Guid>(g.ToString()), g);
		}

		[TestMethod()]
		public void HexToIntTest() {
			Assert.AreEqual(StringCast.HexToInt("e"), 14);
			Assert.AreEqual(StringCast.HexToInt("F"), 15);
			Assert.AreEqual(StringCast.HexToInt("0xF"), 15);
			Assert.AreEqual(StringCast.HexToInt("2F"), 2 * 16 + 15);
			Assert.AreEqual(StringCast.HexToInt("0x2F"), 2 * 16 + 15);
		}

		[TestMethod()]
		public void MaskTest() {
			Assert.AreEqual(StringCast.Mask("abc"), "a**");
			Assert.AreEqual(StringCast.Mask("18888888888"), "188****8888");
			Assert.AreEqual(StringCast.Mask("chenwx521@hotmail.com"), "c********@hotmail.com");
		}

		[TestMethod()]
		public void BetterDisplayCardNumberTest() {
			Assert.AreEqual(StringCast.BetterDisplayCardNumber("1234123412341234"), "1234 1234 1234 1234");
			Assert.AreEqual(StringCast.BetterDisplayCardNumber("1234123412341234123"), "1234 1234 1234 1234 123");
		}
	}
}