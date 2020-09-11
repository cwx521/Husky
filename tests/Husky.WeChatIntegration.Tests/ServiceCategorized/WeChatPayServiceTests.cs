using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.WeChatIntegration.ServiceCategorized.Tests
{
	[TestClass()]
	public class WeChatPayServiceTests
	{

		private readonly string _openId = "ougf3wF_K0LLtG-sVrQELJ615kHk";

		private readonly WeChatAppConfig _wechatConfig = new WeChatAppConfig {
			OpenPlatformAppId = "wx337078d6bc1d9c05",
			OpenPlatformAppSecret = "e4f1aeae78dc0b56a2001dcf9d0f876c",

			MobilePlatformAppId = "wxd67d73189e529060",
			MobilePlatformAppSecret = "45e7ad725a341ddb0e20ca88671b1b0e",

			MiniProgramAppId = "wx0db4db6c8b955ac1",
			MiniProgramAppSecret = "24987a794bfcafb7cee7f4451e19e9f1",

			MerchantId = "1562282191",
			MerchantSecret = "o9PQE3opRQh5KydXq2lBhelrUr47Tz15"
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

			model.TradeType = WeChatPayTradeType.JsApi;
			model.OrderId = OrderIdGen.New();
			var result1 = wechatPay.CreateUnifedOrder(model);
			Assert.IsTrue(result1.Ok);
			Assert.IsNotNull(result1.PrepayId);

			model.TradeType = WeChatPayTradeType.Native;
			model.OrderId = OrderIdGen.New();
			var result2 = wechatPay.CreateUnifedOrder(model);
			Assert.IsTrue(result2.Ok);
			Assert.IsNotNull(result2.PrepayId);
		}

		[TestMethod()]
		public void QueryOrderTest() {

		}

		[TestMethod()]
		public void RefundTest() {

		}

		[TestMethod()]
		public void QueryRefundTest() {

		}
	}
}