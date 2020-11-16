using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class BankCardHelperTests
	{
		[TestMethod()]
		public async Task GetBandCardInfoTest() {
			var bankCard = "5187180804030686";
			var result = await BankCardHelper.GetBandCardInfoAsync(bankCard);
			Assert.AreEqual(BankCardType.CreditCard, result.BankCardType);
			Assert.AreEqual("招商银行", result.BankName);

			var fakeFankCard = "Fake";
			var nullResult = await BankCardHelper.GetBandCardInfoAsync(fakeFankCard);
			Assert.IsNull(nullResult);
		}
	}
}