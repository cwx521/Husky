using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.WeChatIntegration.ServiceCategorized.Tests
{
	[TestClass()]
	public class WeChatPayServiceTests
	{
		public WeChatPayServiceTests() {
			var config = new ConfigurationManager();
			config.AddUserSecrets(GetType().Assembly);
			_wechatConfig = config.GetSection("WeChat").Get<WeChatOptions>();
			_openId = config.GetValue<string>("WeChat:GivenOpenId");
		}

		private readonly string _openId;
		private readonly WeChatOptions _wechatConfig;

		//Need manual input before running test
		private readonly string _newAuthCode = "";

		[TestMethod()]
		public void MicroPayTest() {
			if (string.IsNullOrEmpty(_newAuthCode)) {
				return;
			}

			var times = 2;
			var wxpay = new WeChatPayService(_wechatConfig);
			var model = new WxpayTradeMicroPayModel {
				AppId = _wechatConfig.MobilePlatformAppId,
				Amount = 0.01m * times,
				Body = "Test",
				IPAddress = "127.0.0.1",
				TradeNo = OrderIdGen.New(),
				AuthCode = _newAuthCode
			};
			var payResult = wxpay.MicroPay(model).Result;
			Assert.IsTrue(payResult.Ok);
			Assert.IsFalse(payResult.Data.AwaitPaying);
			Assert.IsNotNull(payResult.Data.TransactionId);
			Assert.AreEqual(model.Amount, payResult.Data.Amount);
			Assert.AreEqual(_openId, payResult.Data.OpenId);

			var refundModel = new WxpayRefundModel {
				AppId = _wechatConfig.MobilePlatformAppId,
				TradeNo = model.TradeNo,
				TotalPaidAmount = model.Amount,
				RefundAmount = 0.01m,
				RefundReason = "UnitTest",
				NewRefundRequestNo = null
			};
			for (int i = 0; i <= times; i++) {
				refundModel.NewRefundRequestNo = $"Refund_{model.TradeNo}_{i}";
				var refundResult = wxpay.RefundAsync(refundModel).Result;

				if (i < times) {
					Assert.IsTrue(refundResult.Ok);
					Assert.AreEqual(refundModel.RefundAmount, refundResult.Data.RefundAmount);
				}
				else {
					Assert.IsFalse(refundResult.Ok);
					Assert.AreEqual("申请退款金额超过订单可退金额", refundResult.Message);
				}
			}
		}

		[TestMethod()]
		public void CreateJsApiPayParameterTest() {
			var fakePrepayId = "Fake";
			var aboutTime = DateTime.Now.Timestamp();

			var wxpay = new WeChatPayService(_wechatConfig);
			var result = wxpay.CreateJsApiPayParameter(fakePrepayId);

			Assert.IsNotNull(result.nonceStr);
			Assert.IsNotNull(result.paySign);
			Assert.IsTrue(Math.Abs(result.timestamp - aboutTime) < 1);
			Assert.AreEqual(fakePrepayId, result.package.RightBy("prepay_id="));
		}

		[TestMethod()]
		public void CreateUnifedOrderTestAsync() {
			var wxpay = new WeChatPayService(_wechatConfig);
			var model = new WxpayTradeCreationModel {
				OpenId = _openId,
				AppId = _wechatConfig.MobilePlatformAppId,
				IPAddress = "127.0.0.1",
				Body = "Test",
				Amount = 0.01m,
				NotifyUrl = "https://zags.xingyisoftware.com/activity/pay/wechat/notify",
			};

			//JsApi
			model.TradeType = WxpayTradeType.JsApi;
			model.TradeNo = OrderIdGen.New();

			var result1 = wxpay.CreateUnifiedOrderAsync(model).Result;
			Assert.IsTrue(result1.Ok);
			Assert.IsNotNull(result1.Data.PrepayId);

			//Native
			model.TradeType = WxpayTradeType.Native;
			model.TradeNo = OrderIdGen.New();

			var result2 = wxpay.CreateUnifiedOrderAsync(model).Result;
			Assert.IsTrue(result2.Ok);
			Assert.IsNotNull(result2.Data.PrepayId);
		}

		[TestMethod()]
		public void CancelOrderTest() {
			var payedTradeNo = "DAEX94019335";

			var wxpay = new WeChatPayService(_wechatConfig);
			var cancelOrderResult = wxpay.CancelTradeAsync(_wechatConfig.MobilePlatformAppId, payedTradeNo, true).Result;
			Assert.IsFalse(cancelOrderResult.Ok);
			Assert.IsTrue(new[] { "已转入退款", "超过订单可撤销时限" }.Contains(cancelOrderResult.Message));
		}

		[TestMethod()]
		public void QueryOrderTestAsync() {
			var payedAmount = 0.05m;
			var payedTradeNo = "DAEX94019335";
			var knownTransactionIdOfThisTrade = "4200067677202301141986028684";

			var wxpay = new WeChatPayService(_wechatConfig);
			var result = wxpay.QueryTradeAsync(_wechatConfig.MobilePlatformAppId, payedTradeNo).Result;
			Assert.IsTrue(result.Ok);
			Assert.IsTrue(result.Data.HasRefund);
			Assert.AreEqual(payedAmount, result.Data.Amount);
			Assert.AreEqual(_openId, result.Data.OpenId);
			Assert.AreEqual(knownTransactionIdOfThisTrade, result.Data.TransactionId);
		}

		[TestMethod()]
		public void RefundTest() {
			var tradeNo = "DAEX94019335";
			var model = new WxpayRefundModel {
				AppId = _wechatConfig.MobilePlatformAppId,
				TradeNo = tradeNo,
				NewRefundRequestNo = "Refund_" + tradeNo,
				TotalPaidAmount = 0.05m,
				RefundAmount = 0.01m,
				RefundReason = "UnitTest"
			};

			var wxpay = new WeChatPayService(_wechatConfig);
			var result = wxpay.RefundAsync(model).Result;
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(model.RefundAmount, result.Data.RefundAmount);
		}

		[TestMethod()]
		public async Task QueryRefundTestAsync() {
			var refundAmount = 0.01m;
			var payedTradeNo = "DAEX94019335";
			var refundRequestNo = "Refund_" + payedTradeNo;

			var wxpay = new WeChatPayService(_wechatConfig);
			var result = await wxpay.QueryRefundAsync(_wechatConfig.MobilePlatformAppId, refundRequestNo);
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(refundAmount, result.Data.RefundAmount);
			Assert.AreEqual(refundAmount, result.Data.AggregatedRefundAmount);
		}
	}
}