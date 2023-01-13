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

		public AlipayOptions Options => _options;
		public IAopClient OriginalClient => _alipay;

		public bool IsAlipayAuthCode(string authCode) =>
			authCode != null &&
			authCode.Length >= 16 &&
			authCode.Length <= 24 &&
			authCode.IsInt64() &&
			new[] { "25", "26", "27", "28", "29", "30" }.Contains(authCode.Substring(0, 2));

		public async Task<AlipayTradeCreationResult> GenerateAlipayPaymentUrlAsync(AlipayTradeCreationModel trade) {
			var payModel = new AlipayTradePayModel {
				Subject = trade.Subject,
				OutTradeNo = trade.TradeNo,
				TotalAmount = trade.Amount.ToString("f2"),
				DisablePayChannels = trade.AllowCreditCard ? null : "credit_group",
				ProductCode = "FAST_INSTANT_TRADE_PAY"
			};

			var wapPayResponse = await Task.Run(() => {
				var wapPayRequest = new AlipayTradeWapPayRequest();
				wapPayRequest.SetNotifyUrl(trade.NotifyUrl ?? _options.DefaultNotifyUrl);
				wapPayRequest.SetReturnUrl(trade.ReturnUrl);
				wapPayRequest.SetBizModel(payModel);
				return _alipay.SdkExecute(wapPayRequest);
			});
			var pagePayResponse = await Task.Run(() => {
				var pagePayRequest = new AlipayTradePagePayRequest();
				pagePayRequest.SetNotifyUrl(trade.NotifyUrl ?? _options.DefaultNotifyUrl);
				pagePayRequest.SetReturnUrl(trade.ReturnUrl);
				pagePayRequest.SetBizModel(payModel);
				return _alipay.SdkExecute(pagePayRequest);
			});

			return new AlipayTradeCreationResult {
				MobileWebPaymentUrl = _options.GatewayUrl + "?" + wapPayResponse.Body,
				DesktopPagePaymentUrl = _options.GatewayUrl + "?" + pagePayResponse.Body
			};
		}

		public async Task<Result<AlipayTradeMicroPayResult>> MicroPayAsync(AlipayTradeMicroPayModel trade) {
			var payModel = new AlipayTradePayModel {
				Subject = trade.Subject,
				OutTradeNo = trade.TradeNo,
				TotalAmount = trade.Amount.ToString("f2"),
				DisablePayChannels = trade.AllowCreditCard ? null : "credit_group",
				AuthCode = trade.AuthCode,
				Scene = trade.Scene,
			};
			var request = new AlipayTradePayRequest();
			request.SetBizModel(payModel);

			return await Task.Run(() => {
				try {
					var response = _alipay.Execute(request);
					var ok = response is { IsError: false, Code: "10000" };

					return new Result<AlipayTradeMicroPayResult> {
						Ok = ok,
						Message = response.SubMsg ?? response.Msg,
						Data = new AlipayTradeMicroPayResult {
							AlipayTradeNo = response?.TradeNo,
							AlipayBuyerId = response?.BuyerUserId,
							Amount = response?.TotalAmount.AsDecimal() ?? 0,
							AwaitPaying = response?.Code == "10003",
							OriginalResult = response
						}
					};
				}
				catch (Exception e) {
					return new Failure<AlipayTradeMicroPayResult>(e.Message);
				}
			});
		}

		public async Task<Result<AlipayTradeQueryResult>> QueryTradeAsync(string tradeNo) {
			var model = new AlipayTradeQueryModel {
				OutTradeNo = tradeNo
			};
			var request = new AlipayTradeQueryRequest();
			request.SetBizModel(model);

			return await Task.Run<Result<AlipayTradeQueryResult>>(() => {
				try {
					var response = _alipay.Execute(request);
					var ok = response is { IsError: false, TradeStatus: "TRADE_SUCCESS" };

					if (!ok) {
						return new Failure<AlipayTradeQueryResult>(response.SubMsg ?? response.Msg);
					}
					return new Success<AlipayTradeQueryResult> {
						Data = new AlipayTradeQueryResult {
							AlipayTradeNo = response.TradeNo,
							AlipayBuyerId = response.BuyerUserId,
							Amount = response.TotalAmount.AsDecimal(),
							OriginalResult = response
						}
					};
				}
				catch (Exception e) {
					return new Failure<AlipayTradeQueryResult>(e.Message);
				}
			});
		}

		public async Task<Result<AlipayRefundResult>> RefundAsync(AlipayRefundModel refund) {
			var model = new AlipayTradeRefundModel {
				OutTradeNo = refund.OriginalTradeNo,
				OutRequestNo = refund.NewRefundRequestNo,
				RefundAmount = refund.RefundAmount.ToString("f2"),
				RefundReason = refund.RefundReason
			};
			var request = new AlipayTradeRefundRequest();
			request.SetBizModel(model);

			return await Task.Run<Result<AlipayRefundResult>>(() => {
				try {
					var response = _alipay.Execute(request);
					var ok = response is { IsError: false, Code: "10000" };

					if (!ok) {
						return new Failure<AlipayRefundResult>(response.SubMsg ?? response.Msg);
					}
					return new Success<AlipayRefundResult> {
						Data = new AlipayRefundResult {
							RefundAmount = ok ? refund.RefundAmount : 0,
							AggregatedRefundAmount = response.RefundFee.AsDecimal(),
							OriginalResult = response
						}
					};
				}
				catch (Exception e) {
					return new Failure<AlipayRefundResult>(e.Message);
				}
			});
		}

		public async Task<Result<AlipayRefundQueryResult>> QueryRefundAsync(string originalTradeNo, string refundRequestNo) {
			var model = new AlipayTradeFastpayRefundQueryModel {
				OutTradeNo = originalTradeNo,
				OutRequestNo = refundRequestNo
			};
			var request = new AlipayTradeFastpayRefundQueryRequest();
			request.SetBizModel(model);

			return await Task.Run<Result<AlipayRefundQueryResult>>(() => {
				try {
					var response = _alipay.Execute(request);
					var ok = response is { IsError: false, RefundStatus: "REFUND_SUCCESS" };

					if (!ok) {
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
				catch (Exception e) {
					return new Failure<AlipayRefundQueryResult>(e.Message);
				}
			});
		}

		public async Task<Result> CancelTradeAsync(string tradeNo, bool allowToCancelEvenAlreadyPaid = false) {
			if (!allowToCancelEvenAlreadyPaid) {
				var queryResult = await QueryTradeAsync(tradeNo);
				if (queryResult.Ok) {
					return new Failure("订单已完成付款，未能撤销");
				}
			}

			var model = new AlipayTradeCancelModel {
				OutTradeNo = tradeNo
			};
			var request = new AlipayTradeCancelRequest();
			request.SetBizModel(model);

			return await Task.Run(() => {
				try {
					var response = _alipay.Execute(request);
					var ok = response is { IsError: false, Code: "10000" };
					return new Result(ok, response.SubMsg ?? response.Msg);
				}
				catch (Exception e) {
					return new Failure(e.Message);
				}
			});
		}

		public async Task<Result<AlipayNotifyResult>> ParseNotifyResultAsync(HttpRequest request) {
			var form = request.HasFormContentType ? await request.ReadFormAsync() : null;
			if (form == null || form.Count == 0) {
				return new Failure<AlipayNotifyResult>("未收到任何参数");
			}

			var dict = form.ToDictionary(k => k.Key, v => v.Value.ToString());
			var statusOk = dict.TryGetValue("trade_status", out var status) && status == "TRADE_SUCCESS";
			var amountOk = dict.TryGetValue("total_amount", out var amount);
			var validationOk = AlipaySignature.RSACheckV1(dict, _options.AlipayPublicKey, _options.CharSet, _options.SignType, false);

			if (!statusOk) {
				return new Failure<AlipayNotifyResult>("支付失败");
			}
			if (!amountOk || !validationOk) {
				return new Failure<AlipayNotifyResult>("未通过数据加密验证");
			}

			return new Success<AlipayNotifyResult> {
				Data = new AlipayNotifyResult {
					TradeNo = form["out_trade_no"],
					AlipayTradeNo = form["trade_no"],
					AlipayBuyerId = form["buyer_id"],
					Amount = amount.AsDecimal(),
					OriginalResult = dict
				}
			};
		}
	}
}
