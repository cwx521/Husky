using System;
using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Husky.Alipay.Models;

namespace Husky.Alipay
{
	public static class AlipayServiceHelper
	{
		public static string GenerateAlipayPaymentUrl(this AlipayService alipay, AlipayPayment payment) {
			var payModel = new AlipayTradePayModel {
				Subject = payment.Subject,
				Body = payment.Body,
				OutTradeNo = payment.OrderId,
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

		public static AlipayOrderQueryResult QueryOrder(this AlipayService alipay, string orderId) {
			var model = new AlipayTradeQueryModel {
				OutTradeNo = orderId
			};
			var request = new AlipayTradeQueryRequest();
			request.SetBizModel(model);

			try {
				var response = alipay.Execute(request);
				return new AlipayOrderQueryResult {
					Ok = !response.IsError && response.Msg == "Success" && response.TradeStatus == "TRADE_SUCCESS",
					Message = response.SubMsg ?? response.Msg,
					Code = response.Code.AsInt(),
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

		public static AlipayRefundResult Refund(this AlipayService alipay, string originalOrderId, string newRefundRequestOrderId, decimal refundAmount, string refundReason) {
			var model = new AlipayTradeRefundModel {
				OutTradeNo = originalOrderId,
				OutRequestNo = newRefundRequestOrderId,
				RefundAmount = refundAmount.ToString("f2"),
				RefundReason = refundReason
			};
			var request = new AlipayTradeRefundRequest();
			request.SetBizModel(model);

			try {
				var response = alipay.Execute(request);
				return new AlipayRefundResult {
					Ok = !response.IsError && response.Msg == "Success",
					Message = response.SubMsg ?? response.Msg,
					Code = response.Code.AsInt(),
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

		public static AlipayRefundQueryResult? QueryRefund(this AlipayService alipay, string originalOrderId, string refundRequestOrderId) {
			var model = new AlipayTradeFastpayRefundQueryModel {
				OutTradeNo = originalOrderId,
				OutRequestNo = refundRequestOrderId
			};
			var request = new AlipayTradeFastpayRefundQueryRequest();
			request.SetBizModel(model);

			try {
				var response = alipay.Execute(request);
				return new AlipayRefundQueryResult {
					Ok = !response.IsError && response.Msg == "Success",
					Message = response.SubMsg ?? response.Msg,
					Code = response.Code.AsInt(),
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
	}
}
