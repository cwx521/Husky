using Husky.WeChatIntegration.ServiceCategorized;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aliyun.Net.SDK.Core;

namespace Husky.WeChatIntegration.ServiceCategorized.Tests
{
	[TestClass()]
	public class WeChatPayServiceTests
	{
		//attention: fill the required values to run this test

		private readonly string _openId = "obrDG51cp7LfrJjXvEe8eFtdvq-4";

		private readonly WeChatOptions _wechatConfig = new WeChatOptions {
			OpenPlatformAppId = "",
			OpenPlatformAppSecret = "",

			MobilePlatformAppId = "",
			MobilePlatformAppSecret = "",

			MiniProgramAppId = "",
			MiniProgramAppSecret = "",

			MerchantId = "",
			MerchantSecret = "",
			MerchantCertFile = @"E:\Documents\Certs\gth_cwxrjgzs_wxp.p12"
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
		public void CreateUnifedOrderTestAsync() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var wechatPay = new WeChatPayService(_wechatConfig);
			var model = new WeChatPayOrderModel {
				OpenId = _openId,
				AppId = _wechatConfig.MobilePlatformAppId,
				IPAddress = "127.0.0.1",
				Body = "Test",
				Amount = 0.01m,
				NotifyUrl = "https://zags.xingyisoftware.com/activity/pay/wechat/notify",
			};

			//JsApi
			model.TradeType = WeChatPayTradeType.JsApi;
			model.OrderNo = OrderIdGen.New();

			var result1 =  wechatPay.CreateUnifedOrderAsync(model).Result;
			Assert.IsTrue(result1.Ok);
			Assert.IsNotNull(result1.Data.PrepayId);

			//Native
			model.TradeType = WeChatPayTradeType.Native;
			model.OrderNo = OrderIdGen.New();

			var result2 =  wechatPay.CreateUnifedOrderAsync(model).Result;
			Assert.IsTrue(result2.Ok);
			Assert.IsNotNull(result2.Data.PrepayId);
		}


		[TestMethod()]
		public void MicroPayTest() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}
			var wechatPay = new WeChatPayService(_wechatConfig);
			var model = new WeChatPayOrderMicroPayModel {
				AppId = _wechatConfig.MobilePlatformAppId,
				Amount = 0.05m,
				Body = "Test",
				IPAddress = "127.0.0.1",
				OrderNo = OrderIdGen.New(),
				AuthCode = "132045584038097537"
			};
			var payResult = wechatPay.MicroPay(model).Result;
			Assert.IsTrue(payResult.Ok);
			Assert.IsFalse(payResult.Data.AwaitPaying);
			Assert.AreEqual(model.Amount, payResult.Data.Amount);
			Assert.AreEqual(_openId, payResult.Data.OpenId);

			var payedAmount = model.Amount;
			var payedOrderNo = model.OrderNo;
			var refundAmount = 0.01m;
			var refundRequestNo = "Refund_" + payedOrderNo;

			var refundResult = wechatPay.RefundAsync(_wechatConfig.MobilePlatformAppId, payedOrderNo, refundRequestNo, payedAmount, refundAmount, "UnitTest").Result;
			Assert.IsTrue(refundResult.Ok);
			Assert.AreEqual(refundAmount, refundResult.Data.RefundAmount);
		}

		[TestMethod()]
		public void CancelOrderTest() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var payedOrderNo = "DACJ13537218";

			var wechatPay = new WeChatPayService(_wechatConfig);
			var cancelOrderResult = wechatPay.CancelOrderAsync(_wechatConfig.MobilePlatformAppId, payedOrderNo, true).Result;
			Assert.IsTrue(cancelOrderResult.Ok);
		}

		[TestMethod()]
		public void QueryOrderTestAsync() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var payedAmount = 0.01m;
			var payedOrderNo = "DACJ13537218";

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = wechatPay.QueryOrderAsync(_wechatConfig.MobilePlatformAppId, payedOrderNo).Result;
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(payedAmount, result.Data.Amount);
			Assert.AreEqual(_openId, result.Data.OpenId);
		}

		[TestMethod()]
		public void RefundTest() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var payedAmount = 0.01m;
			var refundAmount = 0.01m;
			var payedOrderNo = "DACJ13537218";
			var refundRequestNo = "Refund_" + payedOrderNo;

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = wechatPay.RefundAsync(_wechatConfig.MobilePlatformAppId, payedOrderNo, refundRequestNo, payedAmount, refundAmount, "UnitTest").Result;
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(refundAmount, result.Data.RefundAmount);
		}

		[TestMethod()]
		public async Task QueryRefundTestAsync() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var refundAmount = 0.01m;
			var payedOrderNo = "DACJ13537218";
			var refundRequestNo = "Refund_" + payedOrderNo;

			var wechatPay = new WeChatPayService(_wechatConfig);
			var result = await wechatPay.QueryRefundAsync(_wechatConfig.MobilePlatformAppId, refundRequestNo);
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(refundAmount, result.Data.RefundAmount);
		}
	}
}