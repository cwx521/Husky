using System.Diagnostics;
using Alipay.AopSdk.AspnetCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Alipay.Tests
{
	[TestClass()]
	public class AlipayServiceHelperTests
	{
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
		public void GenerateAlipayPaymentUrlTest() {
			if ( string.IsNullOrEmpty(_alipayOptions.PrivateKey) || string.IsNullOrEmpty(_alipayOptions.AlipayPublicKey) ) {
				return;
			}

			var options = Options.Create(_alipayOptions);
			var alipay = new AlipayService(options);

			var tradeModel = new AlipayTradeModel {
				OnMobileDevice = false,
				Amount = 0.1m,
				InternalOrderNo = OrderIdGen.New(),
				Subject = "UnitTest",
				NotifyUrl = "",
				CallbackUrl = "",
			};
			var paymentUrl = alipay.GenerateAlipayPaymentUrl(tradeModel);

			//Payment url is opened up in the default browser
			Process.Start(new ProcessStartInfo(paymentUrl) { UseShellExecute = true });

			//Use debug mode
			//Open the url in browser
			//Set break point here, pay manually in the opened page, then continue
			var queryResult = alipay.QueryOrder(tradeModel.InternalOrderNo);
			Assert.AreEqual(tradeModel.Amount, queryResult.TotalAmount);

			var remainedAmount = tradeModel.Amount;

			var refundAmount = 0.01m;
			var refundRequestOrderNo = OrderIdGen.New();
			var refundResult = alipay.Refund(tradeModel.InternalOrderNo, refundRequestOrderNo, refundAmount, "Test");
			Assert.IsTrue(refundResult.Ok);
			remainedAmount -= refundAmount;

			var queryRefundResult = alipay.QueryRefund(tradeModel.InternalOrderNo, refundRequestOrderNo);
			Assert.AreEqual(refundAmount, queryRefundResult.RefundAmount);

			var queryResult2 = alipay.QueryOrder(tradeModel.InternalOrderNo);
			Assert.AreEqual(tradeModel.Amount, queryResult2.TotalAmount);

			var refundAmount2 = 0.01m;
			var refundResult2 = alipay.Refund(tradeModel.InternalOrderNo, OrderIdGen.New(), refundAmount2, "Test");
			Assert.IsTrue(refundResult2.Ok);
			remainedAmount -= refundAmount2;

			var refundAmount3 = tradeModel.Amount;
			var refundResult3 = alipay.Refund(tradeModel.InternalOrderNo, OrderIdGen.New(), refundAmount3, "Test");
			Assert.IsFalse(refundResult3.Ok);

			var refundResult4 = alipay.Refund(tradeModel.InternalOrderNo, OrderIdGen.New(), remainedAmount, "Test");
			Assert.IsTrue(refundResult4.Ok);
		}
	}
}