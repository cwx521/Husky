using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

			var wxpay = new WeChatPayService(_wechatConfig);
			var result = wxpay.CreateJsApiPayParameter(fakePrepayId);

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

			var result1 = wxpay.CreateUnifedOrderAsync(model).Result;
			Assert.IsTrue(result1.Ok);
			Assert.IsNotNull(result1.Data.PrepayId);

			//Native
			model.TradeType = WxpayTradeType.Native;
			model.TradeNo = OrderIdGen.New();

			var result2 = wxpay.CreateUnifedOrderAsync(model).Result;
			Assert.IsTrue(result2.Ok);
			Assert.IsNotNull(result2.Data.PrepayId);
		}


		[TestMethod()]
		public void MicroPayTest() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}
			var wxpay = new WeChatPayService(_wechatConfig);
			var model = new WxpayTradeMicroPayModel {
				AppId = _wechatConfig.MobilePlatformAppId,
				Amount = 0.05m,
				Body = "Test",
				IPAddress = "127.0.0.1",
				TradeNo = OrderIdGen.New(),
				AuthCode = "132045584038097537"
			};
			var payResult = wxpay.MicroPay(model).Result;
			Assert.IsTrue(payResult.Ok);
			Assert.IsFalse(payResult.Data.AwaitPaying);
			Assert.AreEqual(model.Amount, payResult.Data.Amount);
			Assert.AreEqual(_openId, payResult.Data.OpenId);

			var refundModel = new WxpayRefundModel {
				AppId = _wechatConfig.MobilePlatformAppId,
				TradeNo = model.TradeNo,
				NewRefundRequestNo = "Refund_" + model.TradeNo,
				TotalPaidAmount = model.Amount,
				RefundAmount = 0.01m,
				RefundReason = "UnitTest"
			};
			var refundResult = wxpay.RefundAsync(refundModel).Result;
			Assert.IsTrue(refundResult.Ok);
			Assert.AreEqual(refundModel.RefundAmount, refundResult.Data.RefundAmount);
		}

		[TestMethod()]
		public void CancelOrderTest() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var payedTradeNo = "DACJ13537218";

			var wxpay = new WeChatPayService(_wechatConfig);
			var cancelOrderResult = wxpay.CancelTradeAsync(_wechatConfig.MobilePlatformAppId, payedTradeNo, true).Result;
			Assert.IsTrue(cancelOrderResult.Ok);
		}

		[TestMethod()]
		public void QueryOrderTestAsync() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var payedAmount = 0.01m;
			var payedTradeNo = "DACJ13537218";

			var wxpay = new WeChatPayService(_wechatConfig);
			var result = wxpay.QueryTradeAsync(_wechatConfig.MobilePlatformAppId, payedTradeNo).Result;
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(payedAmount, result.Data.Amount);
			Assert.AreEqual(_openId, result.Data.OpenId);
		}

		[TestMethod()]
		public void RefundTest() {
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var tradeNo = "DACJ13537218";
			var model = new WxpayRefundModel {
				AppId = _wechatConfig.MobilePlatformAppId,
				TradeNo = tradeNo,
				NewRefundRequestNo = "Refund_" + tradeNo,
				TotalPaidAmount = 0.01m,
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
			if (string.IsNullOrEmpty(_wechatConfig.MerchantId) || string.IsNullOrEmpty(_wechatConfig.MerchantSecret)) {
				return;
			}

			var refundAmount = 0.01m;
			var payedTradeNo = "DACJ13537218";
			var refundRequestNo = "Refund_" + payedTradeNo;

			var wxpay = new WeChatPayService(_wechatConfig);
			var result = await wxpay.QueryRefundAsync(_wechatConfig.MobilePlatformAppId, refundRequestNo);
			Assert.IsTrue(result.Ok);
			Assert.AreEqual(refundAmount, result.Data.RefundAmount);
		}
	}
}