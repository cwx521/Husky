using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Alipay.Tests
{
	[TestClass()]
	public class AlipayServiceTests
	{
		private readonly AlipayOptions _alipayOptions = new AlipayOptions {
			AppId = "",
			PrivateKey = "",
			AlipayPublicKey = "",
		};
		private readonly string _oneoffAuthCode = "";

		[TestMethod()]
		public async Task PaymentTransactionTestAsync() {
			if (string.IsNullOrEmpty(_alipayOptions.AppId) ||
				string.IsNullOrEmpty(_alipayOptions.PrivateKey) ||
				string.IsNullOrEmpty(_alipayOptions.AlipayPublicKey)) {
				return;
			}

			var tradeModel = new AlipayOrderPrecreationModel {
				Amount = 0.1m,
				OrderNo = OrderIdGen.New(),
				Subject = "UnitTest"
			};
			var alipay = new AlipayService(_alipayOptions);
			var paymentUrl = (await alipay.GenerateAlipayPaymentUrlAsync(tradeModel)).DesktopPagePaymentUrl;

			//Payment url is opened up in the default browser
			Process.Start(new ProcessStartInfo(paymentUrl) { UseShellExecute = true });

			////////////////////////////////////////////////////////////////////////////////////////
			///// THIS TEST REQUIRES MANUAL OPERATION, USE DEBUG MODE AND SET BREAK POINT HERE /////
			////////////////////////////////////////////////////////////////////////////////////////

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
			Assert.AreEqual(refundAmount, refundResult.Data.RefundAmount);

			remainedAmount -= refundAmount;

			//Query refund 0.01
			var queryRefundResult = await alipay.QueryRefundAsync(tradeModel.OrderNo, refundRequestNo);
			Assert.AreEqual(refundAmount, queryRefundResult.Data.RefundAmount);

			//Refund another 0.01
			var refundAmount2 = 0.01m;
			var refundRequestNo2 = OrderIdGen.New();
			var refundResult2 = await alipay.RefundAsync(tradeModel.OrderNo, refundRequestNo2, refundAmount2, "Test");
			Assert.IsTrue(refundResult2.Ok);
			Assert.AreEqual(refundAmount2, refundResult2.Data.RefundAmount);
			Assert.AreEqual(refundAmount + refundAmount2, refundResult2.Data.AggregatedRefundAmount);

			remainedAmount -= refundAmount2;

			//Query another refund 0.01
			var queryRefundResult2 = await alipay.QueryRefundAsync(tradeModel.OrderNo, refundRequestNo2);
			Assert.AreEqual(refundAmount2, queryRefundResult2.Data.RefundAmount);

			//Expected failure refund
			var refundAmount3 = tradeModel.Amount;
			var refundResult3 = await alipay.RefundAsync(tradeModel.OrderNo, OrderIdGen.New(), refundAmount3, "Test");
			Assert.IsFalse(refundResult3.Ok);
			Assert.IsNotNull(refundResult3.Message);

			//Refund all remained amount
			var refundResult4 = await alipay.RefundAsync(tradeModel.OrderNo, OrderIdGen.New(), remainedAmount, "Test");
			Assert.IsTrue(refundResult4.Ok);
			Assert.AreEqual(tradeModel.Amount, refundResult4.Data.AggregatedRefundAmount);
		}

		[TestMethod()]
		public async Task F2FPayAndCancelOrderTestAsync() {
			if (string.IsNullOrEmpty(_alipayOptions.AppId) ||
				string.IsNullOrEmpty(_alipayOptions.PrivateKey) ||
				string.IsNullOrEmpty(_alipayOptions.AlipayPublicKey)) {
				return;
			}
			if (string.IsNullOrEmpty(_oneoffAuthCode)) {
				return;
			}

			var model = new AlipayOrderF2FPayModel {
				Amount = 0.01m,
				AuthCode = _oneoffAuthCode,
				Subject = "UnitTest",
				OrderNo = OrderIdGen.New()
			};
			var alipay = new AlipayService(_alipayOptions);
			var payResult = await alipay.F2FPayAsync(model);
			Assert.IsTrue(payResult.Ok);
			Assert.AreEqual(model.Amount, payResult.Data.Amount);

			var cancelOrderResult = await alipay.CancelOrderAsync(model.OrderNo, true);
			Assert.IsTrue(cancelOrderResult.Ok);
		}
	}
}