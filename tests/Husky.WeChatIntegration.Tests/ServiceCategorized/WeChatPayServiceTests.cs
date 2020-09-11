using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.WeChatIntegration.ServiceCategorized.Tests
{
	[TestClass()]
	public class WeChatPayServiceTests
	{

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
				OpenId =_openId,
				AppId = _wechatConfig.MobilePlatformAppId,
				IPAddress = "127.0.0.1",
				Body = "Test",
				Amount = 0.01m,
				NotifyUrl = "https://bigfridge.xingyisoftware.com/pay/callback/wechat",
			};

			//JsApi
			model.TradeType = WeChatPayTradeType.JsApi;
			model.OrderId = OrderIdGen.New();

			var result1 = wechatPay.CreateUnifedOrder(model);
			Assert.IsTrue(result1.Ok);
			Assert.IsNotNull(result1.PrepayId);

			//Native
			model.TradeType = WeChatPayTradeType.Native;
			model.OrderId = OrderIdGen.New();

			var result2 = wechatPay.CreateUnifedOrder(model);
			Assert.IsTrue(result2.Ok);
			Assert.IsNotNull(result2.PrepayId);
		}

		[TestMethod()]
		public void QueryOrderTest() {
			var payedOrderId = "DI4365967059199";

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = wechatPay.QueryOrder(_wechatConfig.MobilePlatformAppId, payedOrderId);
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(0.01m, result.Amount);
			Assert.AreEqual(_openId, result.OpenId);
		}

		//[TestMethod()]
		//public void RefundTest() {
		//	var payedOrderId = "[give_an_order_id]";
		//	var newRefundRequestOrderId = "Refund_" + payedOrderId;

		//	var wechatPay = new WeChatPayService(_wechatConfig);
		//	var result = wechatPay.Refund(_wechatConfig.MobilePlatformAppId, payedOrderId, newRefundRequestOrderId, 0.01m, 0.01m, "UnitTest");
		//	Assert.IsTrue(result.Ok);
		//	Assert.AreEqual(0.01m, result.AggregatedRefundAmount);
		//}

		[TestMethod()]
		public void QueryRefundTest() {
			var payedOrderId = "DI4405964098996";
			var refundRequestOrderId = "Refund_" + payedOrderId;

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = wechatPay.QueryRefund(_wechatConfig.MobilePlatformAppId, refundRequestOrderId);
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(0.01m, result.RefundAmount);
		}
	}
}