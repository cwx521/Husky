using System.Diagnostics;
using System.Threading.Tasks;
using Alipay.AopSdk.AspnetCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Alipay.Tests
{
	[TestClass()]
	public class AlipayServiceHelperTests
	{
		//attention: fill the required values to run this test

		private readonly AlipayOptions _alipayOptions = new AlipayOptions {
			Uid = "",
			AppId = "",
			PrivateKey = "",
			AlipayPublicKey = "",
			CharSet = "UTF-8",
			Gatewayurl = "https://openapi.alipay.com/gateway.do",
			SignType = "RSA2"
		};

		[TestMethod()]
		public async Task GenerateAlipayPaymentUrlTestAsync() {
			if ( string.IsNullOrEmpty(_alipayOptions.PrivateKey) || string.IsNullOrEmpty(_alipayOptions.AlipayPublicKey) ) {
				return;
			}

			var options = Options.Create(_alipayOptions);
			var alipay = new AlipayService(options);

			var tradeModel = new AlipayOrderModel {
				Amount = 0.1m,
				OrderNo = OrderIdGen.New(),
				Subject = "UnitTest",
				NotifyUrl = "",
				CallbackUrl = "",
			};
			var paymentUrl = alipay.GenerateAlipayPaymentUrl(tradeModel).DesktopPagePaymentUrl;

			//Payment url is opened up in the default browser
			Process.Start(new ProcessStartInfo(paymentUrl) { UseShellExecute = true });

			//!!!!! THIS TEST REQUIRES MANUAL OPERATION, USE DEBUG MODE AND SET BREAK POINT HERE !!!!
			//Open the url in browser
			//Pay manually in the opened page, then continue
			var queryResult = await alipay.QueryOrderAsync(tradeModel.OrderNo);
			Assert.AreEqual(tradeModel.Amount, queryResult.Data.Amount);

			var remainedAmount = tradeModel.Amount;

			//Refund 0.01
			var refundAmount = 0.01m;
			var refundRequestNo = OrderIdGen.New();
			var refundResult = await alipay.RefundAsync(tradeModel.OrderNo, refundRequestNo, refundAmount, "Test");
			Assert.IsTrue(refundResult.Ok);

			remainedAmount -= refundAmount;

			//Query refund 0.01
			var queryRefundResult = await alipay.QueryRefundAsync(tradeModel.OrderNo, refundRequestNo);
			Assert.AreEqual(refundAmount, queryRefundResult.Data.RefundAmount);

			//Refund another 0.01
			var refundAmount2 = 0.01m;
			var refundRequestNo2 = OrderIdGen.New();
			var refundResult2 = await alipay.RefundAsync(tradeModel.OrderNo, refundRequestNo2, refundAmount2, "Test");
			Assert.IsTrue(refundResult2.Ok);

			remainedAmount -= refundAmount2;

			//Query another refund 0.01
			var queryRefundResult2 = await alipay.QueryRefundAsync(tradeModel.OrderNo, refundRequestNo2);
			Assert.AreEqual(refundAmount2, queryRefundResult2.Data.RefundAmount);

			//Expected failure refund
			var refundAmount3 = tradeModel.Amount;
			var refundResult3 = await alipay.RefundAsync(tradeModel.OrderNo, OrderIdGen.New(), refundAmount3, "Test");
			Assert.IsFalse(refundResult3.Ok);

			//Refund all remained amount
			var refundResult4 = await alipay.RefundAsync(tradeModel.OrderNo, OrderIdGen.New(), remainedAmount, "Test");
			Assert.IsTrue(refundResult4.Ok);
		}
	}
}