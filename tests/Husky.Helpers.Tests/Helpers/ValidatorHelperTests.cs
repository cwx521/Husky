using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class ValidatorHelperTests
	{
		private class TestModel
		{
			[Required]
			[StringLength(10)]
			public string Name { get; set; }

			[Range(6, 21)]
			public int Age { get; set; }
		}


		[TestMethod()]
		public void ValidateTest() {
			var given = new TestModel { Name = "abc", Age = 18 };
			var result = ValidatorHelper.Validate(given);
			Assert.IsTrue(result.Ok);

			given.Name = "aaaaaaaaaaa";
			result = ValidatorHelper.Validate(given);
			Assert.IsFalse(result.Ok);

			given.Name = null;
			result = ValidatorHelper.Validate(given);
			Assert.IsFalse(result.Ok);
		}

		[TestMethod()]
		public void ValidateTest1() {
			var given = new TestModel { Name = "abc", Age = 3 };
			var result = ValidatorHelper.Validate(given, given => given.Name);
			Assert.IsTrue(result.Ok);

			result = ValidatorHelper.Validate(given, given => given.Age);
			Assert.IsFalse(result.Ok);
		}
	}
}