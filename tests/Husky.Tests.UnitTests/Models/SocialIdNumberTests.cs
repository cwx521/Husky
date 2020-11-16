using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class SocialIdNumberTests
	{
		[TestMethod()]
		public void SocialIdNumberTest() {
			var given = "320508200001019889";
			var socialId = new SocialIdNumber(given);

			Assert.IsTrue(socialId.IsValid);
			Assert.AreEqual(DateTime.ParseExact("20000101", "yyyyMMdd", null), socialId.DateOfBirth);
			Assert.AreEqual(DateTime.Now.Year - 2000 + 1, socialId.Age);
			Assert.AreEqual(Sex.Female, socialId.Sex);

			var shouldBeEqual = new SocialIdNumber(given);
			Assert.IsTrue(socialId == shouldBeEqual);
			Assert.IsTrue(socialId.Equals(shouldBeEqual));
		}
	}
}