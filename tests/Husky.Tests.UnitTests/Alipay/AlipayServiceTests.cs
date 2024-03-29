﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Alipay.Tests
{
	[TestClass()]
	public class AlipayServiceTests
	{
		public AlipayServiceTests() {
			var config = new ConfigurationManager();
			config.AddUserSecrets(GetType().Assembly);
			_alipayOptions = config.GetSection("Alipay").Get<AlipayOptions>();
		}

		private readonly AlipayOptions _alipayOptions;

		//Need manual input before running test
		private readonly string _newAuthCode = "";

		[TestMethod()]
		public async Task MicroPayAndCancelOrderTestAsync() {
			if (string.IsNullOrEmpty(_alipayOptions.AppId) ||
				string.IsNullOrEmpty(_alipayOptions.PrivateKey) ||
				string.IsNullOrEmpty(_alipayOptions.AlipayPublicKey)) {
				return;
			}
			if (string.IsNullOrEmpty(_newAuthCode)) {
				return;
			}

			var model = new AlipayTradeMicroPayModel {
				Amount = 0.01m,
				AuthCode = _newAuthCode,
				Subject = "UnitTest",
				TradeNo = OrderIdGen.New()
			};
			var alipay = new AlipayService(_alipayOptions);
			var payResult = await alipay.MicroPayAsync(model);
			Assert.IsTrue(payResult.Ok);
			Assert.AreEqual(model.Amount, payResult.Data.Amount);

			var cancelOrderResult = await alipay.CancelTradeAsync(model.TradeNo, true);
			Assert.IsTrue(cancelOrderResult.Ok);
		}

		[TestMethod()]
		public async Task PaymentTransactionTestAsync() {
			if (string.IsNullOrEmpty(_alipayOptions.AppId) ||
				string.IsNullOrEmpty(_alipayOptions.PrivateKey) ||
				string.IsNullOrEmpty(_alipayOptions.AlipayPublicKey)) {
				return;
			}

			var tradeModel = new AlipayTradeCreationModel {
				Amount = 0.1m,
				TradeNo = OrderIdGen.New(),
				Subject = "UnitTest"
			};
			var alipay = new AlipayService(_alipayOptions);
			var paymentUrl = (await alipay.GenerateAlipayPaymentUrlAsync(tradeModel)).MobileWebPaymentUrl;

			//Payment url is opened up in the default browser
			Process.Start(new ProcessStartInfo(paymentUrl) { UseShellExecute = true });

			////////////////////////////////////////////////////////////////////////////////////////
			///// THIS TEST REQUIRES MANUAL OPERATION, USE DEBUG MODE AND SET BREAK POINT HERE /////
			////////////////////////////////////////////////////////////////////////////////////////

			//Open the url in browser
			//Pay manually in the opened page, then continue
			var queryResult = await alipay.QueryTradeAsync(tradeModel.TradeNo);
			Assert.AreEqual(tradeModel.Amount, queryResult.Data.Amount);

			var totalRefundedAmount = 0m;
			var remainedAmount = tradeModel.Amount;

			//Refund 0.01
			var refundModel = new AlipayRefundModel {
				OriginalTradeNo = tradeModel.TradeNo,
				RefundAmount = 0.01m,
				NewRefundRequestNo = OrderIdGen.New(),
				RefundReason = "RefundUnitTest"
			};
			totalRefundedAmount += refundModel.RefundAmount;
			var refundResult = await alipay.RefundAsync(refundModel);
			Assert.IsTrue(refundResult.Ok);
			Assert.AreEqual(refundModel.RefundAmount, refundResult.Data.RefundAmount);

			remainedAmount -= refundModel.RefundAmount;

			//Query refund 0.01
			var queryRefundResult = await alipay.QueryRefundAsync(tradeModel.TradeNo, refundModel.NewRefundRequestNo);
			Assert.AreEqual(refundModel.RefundAmount, queryRefundResult.Data.RefundAmount);

			//Refund another 0.01
			refundModel.RefundAmount = 0.01m;
			refundModel.NewRefundRequestNo = OrderIdGen.New();
			totalRefundedAmount += refundModel.RefundAmount;
			var refundResult2 = await alipay.RefundAsync(refundModel);
			Assert.IsTrue(refundResult2.Ok);
			Assert.AreEqual(refundModel.RefundAmount, refundResult2.Data.RefundAmount);
			Assert.AreEqual(totalRefundedAmount, refundResult2.Data.AggregatedRefundAmount);

			remainedAmount -= refundModel.RefundAmount;

			//Query another refund 0.01
			var queryRefundResult2 = await alipay.QueryRefundAsync(tradeModel.TradeNo, refundModel.NewRefundRequestNo);
			Assert.AreEqual(refundModel.RefundAmount, queryRefundResult2.Data.RefundAmount);

			//Expected failure refund
			refundModel.RefundAmount = tradeModel.Amount;
			refundModel.NewRefundRequestNo = OrderIdGen.New();
			var refundResult3 = await alipay.RefundAsync(refundModel);
			Assert.IsFalse(refundResult3.Ok);
			Assert.IsNotNull(refundResult3.Message);

			//Refund all remained amount
			refundModel.RefundAmount = remainedAmount;
			refundModel.NewRefundRequestNo = OrderIdGen.New();
			totalRefundedAmount += refundModel.RefundAmount;
			var refundResult4 = await alipay.RefundAsync(refundModel);
			Assert.IsTrue(refundResult4.Ok);
			Assert.AreEqual(tradeModel.Amount, totalRefundedAmount);
			Assert.AreEqual(tradeModel.Amount, refundResult4.Data.AggregatedRefundAmount);
		}
	}
}