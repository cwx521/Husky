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

			var tradeModel = new AlipayPayment {
				OnMobileDevice = false,
				Amount = 0.1m,
				OrderId = OrderIdGen.New(),
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
			var queryResult = alipay.QueryOrder(tradeModel.OrderId);
			Assert.AreEqual(tradeModel.Amount, queryResult.TotalAmount);

			var remainedAmount = tradeModel.Amount;

			//Refund 0.01
			var refundAmount = 0.01m;
			var refundRequestOrderId = OrderIdGen.New();
			var refundResult = alipay.Refund(tradeModel.OrderId, refundRequestOrderId, refundAmount, "Test");
			Assert.IsTrue(refundResult.Ok);

			remainedAmount -= refundAmount;

			//Query refund 0.01
			var queryRefundResult = alipay.QueryRefund(tradeModel.OrderId, refundRequestOrderId);
			Assert.AreEqual(refundAmount, queryRefundResult.RefundAmount);

			//Refund another 0.01
			var refundAmount2 = 0.01m;
			var refundRequestOrderId2 = OrderIdGen.New();
			var refundResult2 = alipay.Refund(tradeModel.OrderId, refundRequestOrderId2, refundAmount2, "Test");
			Assert.IsTrue(refundResult2.Ok);

			remainedAmount -= refundAmount2;

			//Query another refund 0.01
			var queryRefundResult2 = alipay.QueryRefund(tradeModel.OrderId, refundRequestOrderId2);
			Assert.AreEqual(refundAmount2, queryRefundResult2.RefundAmount);

			//Expected failure refund
			var refundAmount3 = tradeModel.Amount;
			var refundResult3 = alipay.Refund(tradeModel.OrderId, OrderIdGen.New(), refundAmount3, "Test");
			Assert.IsFalse(refundResult3.Ok);

			//Refund all remained amount
			var refundResult4 = alipay.Refund(tradeModel.OrderId, OrderIdGen.New(), remainedAmount, "Test");
			Assert.IsTrue(refundResult4.Ok);
		}
	}
}