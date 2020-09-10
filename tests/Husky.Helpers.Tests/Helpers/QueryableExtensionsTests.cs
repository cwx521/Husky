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
			var list = new List<Result<int>>();
			for ( int i = 0; i < 100; i++ ) {
				list.Add(new Result<int> {
					Ok = (i % 3 == 0),
					Message = Guid.NewGuid().ToString(),
					Data = i,
				});
			}
			var queryable = list.AsQueryable();

			var a = queryable.Where(x => x.Ok).ToList();
			var b = queryable.Where(nameof(Result.Ok), "True", Comparison.Equal).ToList();
			var countA = a.Count();
			var countB = b.Count();
			Assert.AreEqual(countA, countB);
			for ( int i = 0; i < Math.Min(countA, countB); i++ ) {
				Assert.AreEqual(a[i].Ok, b[i].Ok);
				Assert.AreEqual(a[i].Message, b[i].Message);
				Assert.AreEqual(a[i].Data, b[i].Data);
			}

			a = queryable.Where(x => x.Data > 80).ToList();
			b = queryable.Where(nameof(Result<int>.Data), 80, Comparison.GreaterThan).ToList();
			countA = a.Count();
			countB = b.Count();
			Assert.AreEqual(countA, countB);
			for ( int i = 0; i < Math.Min(countA, countB); i++ ) {
				Assert.AreEqual(a[i].Ok, b[i].Ok);
				Assert.AreEqual(a[i].Message, b[i].Message);
				Assert.AreEqual(a[i].Data, b[i].Data);
			}

			a = queryable.Where(x => x.Message.Contains("1")).ToList();
			b = queryable.Where(nameof(Result.Message), "1", Comparison.HasKeyword).ToList();
			countA = a.Count();
			countB = b.Count();
			Assert.AreEqual(countA, countB);
			for ( int i = 0; i < Math.Min(countA, countB); i++ ) {
				Assert.AreEqual(a[i].Ok, b[i].Ok);
				Assert.AreEqual(a[i].Message, b[i].Message);
				Assert.AreEqual(a[i].Data, b[i].Data);
			}

			var str = list[new Random().Next(0, list.Count)].Message = "abcde";
			a = queryable.Where(x => x.Message.Length == str.Length).ToList();
			b = queryable.Where("Message.Length", str.Length, Comparison.Equal).ToList();
			countA = a.Count();
			countB = b.Count();
			Assert.AreEqual(countA, 1);
			Assert.AreEqual(countA, countB);
			for ( int i = 0; i < Math.Min(countA, countB); i++ ) {
				Assert.AreEqual(a[i].Ok, b[i].Ok);
				Assert.AreEqual(a[i].Message, b[i].Message);
				Assert.AreEqual(a[i].Data, b[i].Data);
			}
		}

		[TestMethod()]
		public void OrderByTest() {
			var list = new List<Result<int>>();
			for ( int i = 0; i < 100; i++ ) {
				list.Add(new Result<int> {
					Ok = (i % 3 == 0),
					Message = Guid.NewGuid().ToString(),
					Data = i,
				});
			}
			var queryable = list.AsQueryable();

			var a = queryable.OrderBy(x => x.Message).ThenByDescending(x => x.Data).ToList();
			var b = queryable.OrderBy(nameof(Result.Message)).ThenByDescending(nameof(Result<int>.Data)).ToList();
			for ( int i = 0; i < 100; i++ ) {
				Assert.AreEqual(a[i].Ok, b[i].Ok);
				Assert.AreEqual(a[i].Message, b[i].Message);
				Assert.AreEqual(a[i].Data, b[i].Data);
			}
		}
	}
}