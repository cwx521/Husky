//API document: https://opendocs.alipay.com/apis/api_1/alipay.trade.pay

using System;
using System.Linq;
using System.Threading.Tasks;
using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Husky.Alipay.Models;
using Microsoft.AspNetCore.Http;

namespace Husky.Alipay
{
	public static class AlipayServiceHelper
	{
		public static AlipayOrderModelUnifiedResult GenerateAlipayPaymentUrl(this AlipayService alipay, AlipayOrderModel payment) {
			var payModel = new AlipayTradePayModel {
				Subject = payment.Subject,
				Body = payment.Body,
				OutTradeNo = payment.OrderNo,
				TotalAmount = payment.Amount.ToString("f2"),
				ProductCode = "FAST_INSTANT_TRADE_PAY"
			};

			var wapPayRequest = new AlipayTradeWapPayRequest();
			wapPayRequest.SetReturnUrl(payment.CallbackUrl);
			wapPayRequest.SetNotifyUrl(payment.NotifyUrl);
			wapPayRequest.SetBizModel(payModel);
			var wapPayResponse = alipay.SdkExecute(wapPayRequest);

			var pagePayRequest = new AlipayTradePagePayRequest();
			pagePayRequest.SetReturnUrl(payment.CallbackUrl);
			pagePayRequest.SetNotifyUrl(payment.NotifyUrl);
			pagePayRequest.SetBizModel(payModel);
			var pagePayResponse = alipay.SdkExecute(pagePayRequest);

			return new AlipayOrderModelUnifiedResult {
				MobileWebPaymentUrl = alipay.Options.Gatewayurl + "?" + wapPayResponse.Body,
				DesktopPagePaymentUrl = alipay.Options.Gatewayurl + "?" + pagePayResponse.Body
			};
		}

		public static async Task<AlipayOrderQueryResult> QueryOrderAsync(this AlipayService alipay, string orderNo) {
			var model = new AlipayTradeQueryModel {
				OutTradeNo = orderNo
			};
			var request = new AlipayTradeQueryRequest();
			request.SetBizModel(model);

			try {
				var response = await alipay.ExecuteAsync(request);
				return new AlipayOrderQueryResult {
					Ok = !response.IsError && response.Msg == "Success" && response.TradeStatus == "TRADE_SUCCESS",
					Message = response.SubMsg ?? response.Msg,
					AlipayTradeNo = response.TradeNo,
					AlipayBuyerUserId = response.BuyerUserId,
					AlipayBuyerLogonId = response.BuyerLogonId,
					Amount = response.TotalAmount.As<decimal>(),
					OriginalResult = response,
				};
			}
			catch ( Exception e ) {
				return new AlipayOrderQueryResult {
					Ok = false,
					Message = e.Message
				};
			}
		}

		public static async Task<AlipayRefundResult> RefundAsync(this AlipayService alipay, string originalOrderNo, string newRefundRequestNo, decimal refundAmount, string refundReason) {
			var model = new AlipayTradeRefundModel {
				OutTradeNo = originalOrderNo,
				OutRequestNo = newRefundRequestNo,
				RefundAmount = refundAmount.ToString("f2"),
				RefundReason = refundReason
			};
			var request = new AlipayTradeRefundRequest();
			request.SetBizModel(model);

			try {
				var response = await alipay.ExecuteAsync(request);
				var ok = !response.IsError && response.Msg == "Success";
				return new AlipayRefundResult {
					Ok = ok,
					Message = response.SubMsg ?? response.Msg,
					RefundAmount = ok ? refundAmount : 0,
					AggregatedRefundAmount = response.RefundFee.As<decimal>(),
					OriginalResult = response,
				};
			}
			catch ( Exception e ) {
				return new AlipayRefundResult {
					Ok = false,
					Message = e.Message
				};
			}
		}

		public static async Task<AlipayRefundQueryResult> QueryRefundAsync(this AlipayService alipay, string originalOrderNo, string refundRequestNo) {
			var model = new AlipayTradeFastpayRefundQueryModel {
				OutTradeNo = originalOrderNo,
				OutRequestNo = refundRequestNo
			};
			var request = new AlipayTradeFastpayRefundQueryRequest();
			request.SetBizModel(model);

			try {
				var response = await alipay.ExecuteAsync(request);
				return new AlipayRefundQueryResult {
					Ok = !response.IsError && response.Msg == "Success",
					Message = response.SubMsg ?? response.Msg,
					RefundReason = response.RefundReason,
					RefundAmount = response.RefundAmount.As<decimal>(),
					OriginalResult = response,
				};
			}
			catch ( Exception e ) {
				return new AlipayRefundQueryResult {
					Ok = false,
					Message = e.Message
				};
			}
		}

		public static async Task<AlipayNotifyResult> ParseNotifyResultAsync(this AlipayService alipay, HttpRequest request) {
			var form = request.HasFormContentType ? await request.ReadFormAsync() : null;
			if ( form == null || form.Count == 0 ) {
				return new AlipayNotifyResult { Ok = false, Message = "未收到任何参数" };
			}

			var success = form.TryGetValue("trade_status", out var status) && status == "TRADE_SUCCESS";
			if ( !success ) {
				return new AlipayNotifyResult { Ok = false, Message = "支付失败" };
			}

			var dict = form.ToDictionary(k => k.Key, v => v.Value.ToString());
			var validationOk = dict.TryGetValue("total_amount", out var amount) && alipay.RSACheckV1(dict);
			if ( !validationOk ) {
				return new AlipayNotifyResult { Ok = false, Message = "未通过数据加密验证" };
			}

			return new AlipayNotifyResult {
				Ok = true,
				OrderNo = form["out_trade_no"],
				AlipayTradeNo = form["trade_no"],
				AlipayBuyerId = form["buyer_id"],
				Amount = amount.As<decimal>(),
				OriginalResult = dict
			};
		}
	}
}
