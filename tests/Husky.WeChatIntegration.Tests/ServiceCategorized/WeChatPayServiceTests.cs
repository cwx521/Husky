using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.WeChatIntegration.ServiceCategorized.Tests
{
	[TestClass()]
	public class WeChatPayServiceTests
	{
		//attention: fill the required values to run this test

		private readonly string _openId = "ougf3wF_K0LLtG-sVrQELJ615kHk";

		private readonly WeChatAppConfig _wechatConfig = new WeChatAppConfig {
			OpenPlatformAppId = "",
			OpenPlatformAppSecret = "",

			MobilePlatformAppId = "",
			MobilePlatformAppSecret = "",

			MiniProgramAppId = "",
			MiniProgramAppSecret = "",

			MerchantId = "",
			MerchantSecret = ""
		};

		[TestMethod()]
		public void CreateJsApiPayParameterTest() {
			var fakePrepayId = "Fake";
			var aboutTime = DateTime.Now.Timestamp();

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = wechatPay.CreateJsApiPayParameter(fakePrepayId);

			Assert.IsNotNull(result.nonceStr);
			Assert.IsNotNull(result.paySign);
			Assert.IsTrue(Math.Abs(result.timestamp - aboutTime) < 1);
			Assert.AreEqual(fakePrepayId, result.package.RightBy("prepay_id="));
		}

		[TestMethod()]
		public void CreateUnifedOrderTest() {
			if ( string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret) ) {
				return;
			}

			var wechatPay = new WeChatPayService(_wechatConfig);
			var model = new WeChatPayOrderModel {
				OpenId = _openId,
				AppId = _wechatConfig.MobilePlatformAppId,
				IPAddress = "127.0.0.1",
				Body = "Test",
				Amount = 0.01m,
				NotifyUrl = "https://bigfridge.xingyisoftware.com/pay/callback/wechat",
			};

			//JsApi
			model.TradeType = WeChatPayTradeType.JsApi;
			model.OrderNo = OrderIdGen.New();

			var result1 = wechatPay.CreateUnifedOrder(model);
			Assert.IsTrue(result1.Ok);
			Assert.IsNotNull(result1.PrepayId);

			//Native
			model.TradeType = WeChatPayTradeType.Native;
			model.OrderNo = OrderIdGen.New();

			var result2 = wechatPay.CreateUnifedOrder(model);
			Assert.IsTrue(result2.Ok);
			Assert.IsNotNull(result2.PrepayId);
		}

		[TestMethod()]
		public void QueryOrderTest() {
			if ( string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret) ) {
				return;
			}

			var payedOrderNo = "DIB710795325592";   //DI4365967059199

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = wechatPay.QueryOrder(_wechatConfig.MobilePlatformAppId, payedOrderNo);
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(0.01m, result.Amount);
			Assert.AreEqual(_openId, result.OpenId);
		}

		[TestMethod()]
		public void RefundTest() {
			if ( string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret) ) {
				return;
			}

			var payedOrderNo = "DIB710795325592";
			var newRefundRequestNo = OrderIdGen.New();

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = wechatPay.Refund(_wechatConfig.MobilePlatformAppId, payedOrderNo, newRefundRequestNo, 0.1m, 0.01m, "UnitTest");
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(0.01m, result.RefundAmount);
		}

		[TestMethod()]
		public void QueryRefundTest() {
			if ( string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret) ) {
				return;
			}

			var payedOrderNo = "DIB710795325592";
			var refundRequestNo = "Refund_" + payedOrderNo;

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = wechatPay.QueryRefund(_wechatConfig.MobilePlatformAppId, refundRequestNo);
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(0.01m, result.RefundAmount);
		}
	}
}