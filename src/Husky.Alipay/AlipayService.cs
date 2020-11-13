using System;
using System.Linq;
using System.Threading.Tasks;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Util;
using Microsoft.AspNetCore.Http;

namespace Husky.Alipay
{
	public class AlipayService
	{
		public AlipayService(AlipayOptions options) {
			_options = options;
			_alipay = new DefaultAopClient(
				options.GatewayUrl,
				options.AppId,
				options.PrivateKey,
				options.Format,
				options.Version,
				options.SignType,
				options.AlipayPublicKey,
				options.CharSet
			);
		}

		private readonly AlipayOptions _options;
		private readonly DefaultAopClient _alipay;

		public IAopClient OriginalClient => _alipay;

		public AlipayOrderCreationResult GenerateAlipayPaymentUrl(AlipayOrderModel payment) {
			var payModel = new AlipayTradePayModel {
				Subject = payment.Subject,
				Body = payment.Body,
				OutTradeNo = payment.OrderNo,
				TotalAmount = payment.Amount.ToString("f2"),
				DisablePayChannels = payment.AllowCreditCard ? null : "credit_group",
				ProductCode = "FAST_INSTANT_TRADE_PAY"
			};

			var wapPayRequest = new AlipayTradeWapPayRequest();
			wapPayRequest.SetNotifyUrl(payment.NotifyUrl ?? _options.DefaultNotifyUrl);
			wapPayRequest.SetReturnUrl(payment.CallbackUrl);
			wapPayRequest.SetBizModel(payModel);
			var wapPayResponse = _alipay.SdkExecute(wapPayRequest);

			var pagePayRequest = new AlipayTradePagePayRequest();
			pagePayRequest.SetNotifyUrl(payment.NotifyUrl ?? _options.DefaultNotifyUrl);
			pagePayRequest.SetReturnUrl(payment.CallbackUrl);
			pagePayRequest.SetBizModel(payModel);
			var pagePayResponse = _alipay.SdkExecute(pagePayRequest);

			return new AlipayOrderCreationResult {
				MobileWebPaymentUrl = _options.GatewayUrl + "?" + wapPayResponse.Body,
				DesktopPagePaymentUrl = _options.GatewayUrl + "?" + pagePayResponse.Body
			};
		}

		public Result<AlipayOrderQueryResult> QueryOrder(string orderNo) {
			var model = new AlipayTradeQueryModel {
				OutTradeNo = orderNo
			};
			var request = new AlipayTradeQueryRequest();
			request.SetBizModel(model);

			try {
				var response = _alipay.Execute(request);
				var ok = response is { IsError: false, Msg: "Success", TradeStatus: "TRADE_SUCCESS" };

				if ( !ok ) {
					return new Failure<AlipayOrderQueryResult>(response.SubMsg ?? response.Msg);
				}
				return new Success<AlipayOrderQueryResult> {
					Data = new AlipayOrderQueryResult {
						AlipayTradeNo = response.TradeNo,
						AlipayBuyerUserId = response.BuyerUserId,
						AlipayBuyerLogonId = response.BuyerLogonId,
						Amount = response.TotalAmount.AsDecimal(),
						OriginalResult = response
					}
				};
			}
			catch ( Exception e ) {
				return new Failure<AlipayOrderQueryResult>(e.Message);
			}
		}

		public Result<AlipayRefundResult> Refund(string originalOrderNo, string newRefundRequestNo, decimal refundAmount, string refundReason) {
			var model = new AlipayTradeRefundModel {
				OutTradeNo = originalOrderNo,
				OutRequestNo = newRefundRequestNo,
				RefundAmount = refundAmount.ToString("f2"),
				RefundReason = refundReason
			};
			var request = new AlipayTradeRefundRequest();
			request.SetBizModel(model);

			try {
				var response = _alipay.Execute(request);
				var ok = response is { IsError: false, Msg: "Success" };

				if ( !ok ) {
					return new Failure<AlipayRefundResult>(response.SubMsg ?? response.Msg);
				}
				return new Success<AlipayRefundResult> {
					Data = new AlipayRefundResult {
						RefundAmount = ok ? refundAmount : 0,
						AggregatedRefundAmount = response.RefundFee.AsDecimal(),
						OriginalResult = response
					}
				};
			}
			catch ( Exception e ) {
				return new Failure<AlipayRefundResult>(e.Message);
			}
		}

		public Result<AlipayRefundQueryResult> QueryRefund(string originalOrderNo, string refundRequestNo) {
			var model = new AlipayTradeFastpayRefundQueryModel {
				OutTradeNo = originalOrderNo,
				OutRequestNo = refundRequestNo
			};
			var request = new AlipayTradeFastpayRefundQueryRequest();
			request.SetBizModel(model);

			try {
				var response = _alipay.Execute(request);
				var ok = response is { IsError: false, Msg: "Success" };

				if ( !ok ) {
					return new Failure<AlipayRefundQueryResult>(response.SubMsg ?? response.Msg);
				}
				return new Success<AlipayRefundQueryResult> {
					Data = new AlipayRefundQueryResult {
						RefundReason = response.RefundReason,
						RefundAmount = response.RefundAmount.AsDecimal(),
						OriginalResult = response
					}
				};
			}
			catch ( Exception e ) {
				return new Failure<AlipayRefundQueryResult>(e.Message);
			}
		}

		public async Task<Result<AlipayNotifyResult>> ParseNotifyResultAsync(HttpRequest request) {
			var form = request.HasFormContentType ? await request.ReadFormAsync() : null;
			if ( form == null || form.Count == 0 ) {
				return new Failure<AlipayNotifyResult>("未收到任何参数");
			}

			var dict = form.ToDictionary(k => k.Key, v => v.Value.ToString());
			var success = dict.TryGetValue("trade_status", out var status) && status == "TRADE_SUCCESS";
			var readAmount = dict.TryGetValue("total_amount", out var amount);
			var validationOk = AlipaySignature.RSACheckV1(dict, _options.AlipayPublicKey, _options.CharSet, _options.SignType, false);

			if ( !success ) {
				return new Failure<AlipayNotifyResult>("支付失败");
			}
			if ( !readAmount || !validationOk ) {
				return new Failure<AlipayNotifyResult>("未通过数据加密验证");
			}

			return new Success<AlipayNotifyResult> {
				Data = new AlipayNotifyResult {
					OrderNo = form["out_trade_no"],
					AlipayTradeNo = form["trade_no"],
					AlipayBuyerId = form["buyer_id"],
					Amount = amount.AsDecimal(),
					OriginalResult = dict
				}
			};
		}
	}
}
