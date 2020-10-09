//API document: https://opendocs.alipay.com/apis/api_1/alipay.trade.pay

using System;
using System.Collections.Generic;
using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Husky.Alipay.Models;
using Microsoft.AspNetCore.Http;

namespace Husky.Alipay
{
	public static class AlipayServiceHelper
	{
		public static string GenerateAlipayPaymentUrl(this AlipayService alipay, AlipayPayment payment) {
			var payModel = new AlipayTradePayModel {
				Subject = payment.Subject,
				Body = payment.Body,
				OutTradeNo = payment.OrderNo,
				TotalAmount = payment.Amount.ToString("f2"),
				ProductCode = "FAST_INSTANT_TRADE_PAY"
			};

			if ( payment.OnMobileDevice ) {
				var request = new AlipayTradeWapPayRequest();
				request.SetReturnUrl(payment.CallbackUrl);
				request.SetNotifyUrl(payment.NotifyUrl);
				request.SetBizModel(payModel);

				var response = alipay.SdkExecute(request);
				return alipay.Options.Gatewayurl + "?" + response.Body;
			}
			else {
				var request = new AlipayTradePagePayRequest();
				request.SetReturnUrl(payment.CallbackUrl);
				request.SetNotifyUrl(payment.NotifyUrl);
				request.SetBizModel(payModel);

				var response = alipay.SdkExecute(request);
				return alipay.Options.Gatewayurl + "?" + response.Body;
			}
		}

		public static AlipayOrderQueryResult QueryOrder(this AlipayService alipay, string orderNo) {
			var model = new AlipayTradeQueryModel {
				OutTradeNo = orderNo
			};
			var request = new AlipayTradeQueryRequest();
			request.SetBizModel(model);

			try {
				var response = alipay.Execute(request);
				return new AlipayOrderQueryResult {
					Ok = !response.IsError && response.Msg == "Success" && response.TradeStatus == "TRADE_SUCCESS",
					Message = response.SubMsg ?? response.Msg,
					AlipayTradeNo = response.TradeNo,
					AlipayBuyerUserId = response.BuyerUserId,
					AlipayBuyerLogonId = response.BuyerLogonId,
					TotalAmount = response.TotalAmount.As<decimal>(),
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

		public static AlipayRefundResult Refund(this AlipayService alipay, string originalOrderNo, string newRefundRequestNo, decimal refundAmount, string refundReason) {
			var model = new AlipayTradeRefundModel {
				OutTradeNo = originalOrderNo,
				OutRequestNo = newRefundRequestNo,
				RefundAmount = refundAmount.ToString("f2"),
				RefundReason = refundReason
			};
			var request = new AlipayTradeRefundRequest();
			request.SetBizModel(model);

			try {
				var response = alipay.Execute(request);
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

		public static AlipayRefundQueryResult? QueryRefund(this AlipayService alipay, string originalOrderNo, string refundRequestNo) {
			var model = new AlipayTradeFastpayRefundQueryModel {
				OutTradeNo = originalOrderNo,
				OutRequestNo = refundRequestNo
			};
			var request = new AlipayTradeFastpayRefundQueryRequest();
			request.SetBizModel(model);

			try {
				var response = alipay.Execute(request);
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

		public static AlipayNotifyResult ParseNotifyResult(this AlipayService alipay, HttpRequest request) {
			var dict = new Dictionary<string, string>();

			if ( request.HasFormContentType ) {
				foreach ( var i in request.Form.Keys ) dict.Add(i, request.Form[i]);
			}
			else if ( request.Query.Count != 0 ) {
				foreach ( var i in request.Query.Keys ) dict.Add(i, request.Query[i]);
			}

			if ( dict.Count == 0 ) {
				return new AlipayNotifyResult { Ok = false, Message = "未收到任何参数" };
			}

			var success = dict.TryGetValue("trade_status", out var status) && (status == "TRADE_SUCCESS" || status == "TRADE_FINISHED");
			if ( !success ) {
				return new AlipayNotifyResult { Ok = false, Message = "支付失败" };
			}

			var validationOk = dict.TryGetValue("trade_status", out var amount) && alipay.RSACheckV1(dict);
			if ( !validationOk ) {
				return new AlipayNotifyResult { Ok = false, Message = "未通过数据加密验证" };
			}

			return new AlipayNotifyResult {
				Ok = true,
				OrderNo = dict["out_trade_no"],
				AlipayTradeNo = dict["trade_no"],
				AlipayBuyerId = dict["buyer_id"],
				Amount = amount.As<decimal>()
			};
		}
	}
}
