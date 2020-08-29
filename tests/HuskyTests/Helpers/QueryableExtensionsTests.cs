using System;
using System.Collections.Generic;
using System.Linq;
using Husky;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class QueryableExtensionsTests
	{
		[TestMethod()]
		public void WhereTest() {
			var list = new List<Result>();
			for ( int i = 0; i < 100; i++ ) {
				list.Add(new Result {
					Ok = (i % 3 == 0),
					Message = Guid.NewGuid().ToString(),
					Code = i,
				});
			}
			var queryable = list.AsQueryable();

			var a = queryable.Where(x => x.Ok);
			var b = queryable.Where(nameof(Result.Ok), "True", Comparison.Equal);
			var countA = a.Count();
			var countB = b.Count();
			Assert.AreEqual(countA, countB);
			for ( int i = 0; i < Math.Min(countA, countB); i++ ) {
				Assert.AreEqual(a.ElementAt(i).Ok, b.ElementAt(i).Ok);
				Assert.AreEqual(a.ElementAt(i).Message, b.ElementAt(i).Message);
				Assert.AreEqual(a.ElementAt(i).Code, b.ElementAt(i).Code);
			}

			a = queryable.Where(x => x.Code > 80);
			b = queryable.Where(nameof(Result.Code), 80, Comparison.GreaterThan);
			countA = a.Count();
			countB = b.Count();
			Assert.AreEqual(countA, countB);
			for ( int i = 0; i < Math.Min(countA, countB); i++ ) {
				Assert.AreEqual(a.ElementAt(i).Ok, b.ElementAt(i).Ok);
				Assert.AreEqual(a.ElementAt(i).Message, b.ElementAt(i).Message);
				Assert.AreEqual(a.ElementAt(i).Code, b.ElementAt(i).Code);
			}

			a = queryable.Where(x => x.Message.Contains("1"));
			b = queryable.Where(nameof(Result.Message), "1", Comparison.HasKeyword);
			countA = a.Count();
			countB = b.Count();
			Assert.AreEqual(countA, countB);
			for ( int i = 0; i < Math.Min(countA, countB); i++ ) {
				Assert.AreEqual(a.ElementAt(i).Ok, b.ElementAt(i).Ok);
				Assert.AreEqual(a.ElementAt(i).Message, b.ElementAt(i).Message);
				Assert.AreEqual(a.ElementAt(i).Code, b.ElementAt(i).Code);
			}

			var str = list[new Random().Next(0, list.Count)].Message = "abcde";
			a = queryable.Where(x => x.Message.Length == str.Length);
			b = queryable.Where("Message.Length", str.Length, Comparison.Equal);
			countA = a.Count();
			countB = b.Count();
			Assert.AreEqual(countA, 1);
			Assert.AreEqual(countA, countB);
			for ( int i = 0; i < Math.Min(countA, countB); i++ ) {
				Assert.AreEqual(a.ElementAt(i).Ok, b.ElementAt(i).Ok);
				Assert.AreEqual(a.ElementAt(i).Message, b.ElementAt(i).Message);
				Assert.AreEqual(a.ElementAt(i).Code, b.ElementAt(i).Code);
			}
		}

		[TestMethod()]
		public void OrderByTest() {
			var list = new List<Result>();
			for ( int i = 0; i < 100; i++ ) {
				list.Add(new Result {
					Ok = (i % 3 == 0),
					Message = Guid.NewGuid().ToString(),
					Code = i,
				});
			}
			var queryable = list.AsQueryable();

			var a = queryable.OrderBy(x => x.Message).ThenByDescending(x => x.Code);
			var b = queryable.OrderBy(nameof(Result.Message)).ThenByDescending(nameof(Result.Code));
			for ( int i = 0; i < 100; i++ ) {
				Assert.AreEqual(a.ElementAt(i).Ok, b.ElementAt(i).Ok);
				Assert.AreEqual(a.ElementAt(i).Message, b.ElementAt(i).Message);
				Assert.AreEqual(a.ElementAt(i).Code, b.ElementAt(i).Code);
			}
		}
	}
}