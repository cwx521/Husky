using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Alipay.Tests
{
	[TestClass()]
	public class AlipayServiceTests
	{
		//attention: fill the required values to run this test

		private readonly AlipayOptions _alipayOptions = new AlipayOptions {
			PartnerId = "",
			AppId = "",
			PrivateKey = "",
			AlipayPublicKey = "",
		};

		[TestMethod()]
		public void PaymentTransactionTest() {
			if ( string.IsNullOrEmpty(_alipayOptions.PrivateKey) || string.IsNullOrEmpty(_alipayOptions.AlipayPublicKey) ) {
				return;
			}

			var alipay = new AlipayService(_alipayOptions);
			var tradeModel = new AlipayOrderModel {
				Amount = 0.1m,
				OrderNo = OrderIdGen.New(),
				Subject = "UnitTest"
			};
			var paymentUrl = alipay.GenerateAlipayPaymentUrl(tradeModel).DesktopPagePaymentUrl;

			//Payment url is opened up in the default browser
			Process.Start(new ProcessStartInfo(paymentUrl) { UseShellExecute = true });

			////////////////////////////////////////////////////////////////////////////////////////
			///// THIS TEST REQUIRES MANUAL OPERATION, USE DEBUG MODE AND SET BREAK POINT HERE /////
			////////////////////////////////////////////////////////////////////////////////////////

			//Open the url in browser
			//Pay manually in the opened page, then continue
			var queryResult = alipay.QueryOrder(tradeModel.OrderNo);
			Assert.AreEqual(tradeModel.Amount, queryResult.Data.Amount);

			var remainedAmount = tradeModel.Amount;

			//Refund 0.01
			var refundAmount = 0.01m;
			var refundRequestNo = OrderIdGen.New();
			var refundResult = alipay.Refund(tradeModel.OrderNo, refundRequestNo, refundAmount, "Test");
			Assert.IsTrue(refundResult.Ok);
			Assert.AreEqual(refundAmount, refundResult.Data.RefundAmount);

			remainedAmount -= refundAmount;

			//Query refund 0.01
			var queryRefundResult = alipay.QueryRefund(tradeModel.OrderNo, refundRequestNo);
			Assert.AreEqual(refundAmount, queryRefundResult.Data.RefundAmount);

			//Refund another 0.01
			var refundAmount2 = 0.01m;
			var refundRequestNo2 = OrderIdGen.New();
			var refundResult2 = alipay.Refund(tradeModel.OrderNo, refundRequestNo2, refundAmount2, "Test");
			Assert.IsTrue(refundResult2.Ok);
			Assert.AreEqual(refundAmount2, refundResult2.Data.RefundAmount);
			Assert.AreEqual(refundAmount + refundAmount2, refundResult2.Data.AggregatedRefundAmount);

			remainedAmount -= refundAmount2;

			//Query another refund 0.01
			var queryRefundResult2 = alipay.QueryRefund(tradeModel.OrderNo, refundRequestNo2);
			Assert.AreEqual(refundAmount2, queryRefundResult2.Data.RefundAmount);

			//Expected failure refund
			var refundAmount3 = tradeModel.Amount;
			var refundResult3 = alipay.Refund(tradeModel.OrderNo, OrderIdGen.New(), refundAmount3, "Test");
			Assert.IsFalse(refundResult3.Ok);
			Assert.IsNotNull(refundResult3.Message);

			//Refund all remained amount
			var refundResult4 = alipay.Refund(tradeModel.OrderNo, OrderIdGen.New(), remainedAmount, "Test");
			Assert.IsTrue(refundResult4.Ok);
			Assert.AreEqual(tradeModel.Amount, refundResult4.Data.AggregatedRefundAmount);
		}
	}
}