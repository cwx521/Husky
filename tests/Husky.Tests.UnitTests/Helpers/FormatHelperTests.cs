using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class FormatHelperTests
	{
		[TestMethod()]
		public void TrimEndTest() {
			Assert.AreEqual("12", FormatHelper.TrimEnd(12.00m));
			Assert.AreEqual("12.345", FormatHelper.TrimEnd(12.345m));
			Assert.AreEqual("12.3", 12.30m.TrimEnd() );
			Assert.AreEqual("12.03", 12.03m.TrimEnd());
			Assert.AreEqual("12.03", 12.030m.TrimEnd());
		}
	}
}