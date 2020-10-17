using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class BankCardHelperTests
	{
		[TestMethod()]
		public void GetBandCardInfoTest() {
			var bankCard = "5187180804030686";
			var result = BankCardHelper.GetBandCardInfo(bankCard).Result;
			Assert.AreEqual(BankCardType.CreditCard, result.BankCardType);
			Assert.AreEqual("招商银行", result.BankName);

			var fakeFankCard = "Fake";
			var nullResult = BankCardHelper.GetBandCardInfo(fakeFankCard).Result;
			Assert.IsNull(nullResult);
		}
	}
}