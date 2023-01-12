//API document: https://pay.weixin.qq.com/wiki/doc/api/index.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Husky.WeChatIntegration.Models.Pay;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatPayService
	{
		public WeChatPayService(WeChatOptions options) {
			_options = options;
			_options.RequireMerchantSettings();
		}

		private readonly WeChatOptions _options;

		public bool IsWeChatPayAuthCode(string authCode) =>
			authCode != null &&
			authCode.Length == 18 &&
			authCode.IsInt64() &&
			new[] { "10", "11", "12", "13", "14", "15" }.Contains(authCode.Substring(0, 2));

		public WeChatPayJsApiParameter CreateJsApiPayParameter(string prepayId) {
			var nonceStr = Crypto.RandomString(32);
			var timeStamp = DateTime.Now.Timestamp();

			var sb = new StringBuilder();
			sb.Append("appId=" + _options.MobilePlatformAppId);
			sb.Append("&nonceStr=" + nonceStr);
			sb.Append("&package=prepay_id=" + prepayId);
			sb.Append("&signType=MD5");
			sb.Append("&timeStamp=" + timeStamp);
			sb.Append("&key=" + _options.MerchantSecret);
			var paySign = Crypto.MD5(sb.ToString()).ToUpper();

			return new WeChatPayJsApiParameter {
				timestamp = timeStamp,
				nonceStr = nonceStr,
				package = $"prepay_id={prepayId}",
				signType = "MD5",
				paySign = paySign
			};
		}

		public async Task<Result<WeChatPayOrderCreationResult>> CreateUnifedOrderAsync(WeChatPayOrderModel model) {
			const string apiUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";

			var now = DateTime.Now;
			var parameters = GetCommonParameters(model.AppId);
			var more = new Dictionary<string, string> {
				{ "time_start", now.ToString("yyyyMMddHHmmss") },
				{ "time_expire", now.Add(model.Expiration).ToString("yyyyMMddHHmmss") },
				{ "device_info", model.Device },
				{ "trade_type", model.TradeType.ToUpper() },
				{ "spbill_create_ip", model.IPAddress },
				{ "out_trade_no", model.OrderNo },
				{ "notify_url", model.NotifyUrl },
				{ "openid", model.OpenId },
				{ "total_fee", (model.Amount * 100).ToString("f0") },
				{ "body", model.Body },
			};
			foreach (var i in more) {
				parameters.Add(i.Key, i.Value);
			}
			if (!model.AllowCreditCard) {
				parameters.Add("limit_pay", "no_credit");
			}
			if (!string.IsNullOrEmpty(model.Attach)) {
				parameters.Add("attach", model.Attach);
			}

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters);

				if (!IsResultCodeSuccess(xml)) {
					return new Failure<WeChatPayOrderCreationResult>(GetErrorDescription(xml));
				}
				return new Success<WeChatPayOrderCreationResult> {
					Data = new WeChatPayOrderCreationResult {
						PrepayId = GetContent(xml, "prepay_id"),
						CodeUrl = GetContent(xml, "code_url"),
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatPayOrderCreationResult>(e.Message);
			};
		}

		public async Task<Result<WeChatPayOrderMicroPayResult>> MicroPay(WeChatPayOrderMicroPayModel model) {
			const string apiUrl = "https://api.mch.weixin.qq.com/pay/micropay";

			var now = DateTime.Now;
			var parameters = GetCommonParameters(model.AppId);
			var more = new Dictionary<string, string> {
				{ "time_start", now.ToString("yyyyMMddHHmmss") },
				{ "time_expire", now.Add(model.Expiration).ToString("yyyyMMddHHmmss") },
				{ "spbill_create_ip", model.IPAddress },
				{ "out_trade_no", model.OrderNo },
				{ "total_fee", (model.Amount * 100).ToString("f0") },
				{ "body", model.Body },
				{ "auth_code", model.AuthCode },
			};
			foreach (var i in more) {
				parameters.Add(i.Key, i.Value);
			}
			if (!model.AllowCreditCard) {
				parameters.Add("limit_pay", "no_credit");
			}
			if (!string.IsNullOrEmpty(model.Attach)) {
				parameters.Add("attach", model.Attach);
			}

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters);
				return new Result<WeChatPayOrderMicroPayResult> {
					Ok = IsResultCodeSuccess(xml) && GetContent(xml, "trade_type") == "MICROPAY",
					Message = GetErrorDescription(xml),
					Data = new WeChatPayOrderMicroPayResult {
						OpenId = GetContent(xml, "openid"),
						TransactionId = GetContent(xml, "transaction_id"),
						Amount = GetValue<int>(xml, "total_fee") / 100m,
						AwaitPaying = GetContent(xml, "err_code") == "USERPAYING",
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatPayOrderMicroPayResult>(e.Message);
			};
		}

		public async Task<Result<WeChatPayOrderQueryResult>> QueryOrderAsync(string appId, string orderNo) {
			const string apiUrl = "https://api.mch.weixin.qq.com/pay/orderquery";

			var parameters = GetCommonParameters(appId);
			parameters.Add("out_trade_no", orderNo);

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters);
				var tradeState = GetContent(xml, "trade_state");

				if (tradeState != "REFUND" && tradeState != "SUCCESS") {
					return new Failure<WeChatPayOrderQueryResult>(GetErrorDescription(xml));
				}

				return new Success<WeChatPayOrderQueryResult> {
					Data = new WeChatPayOrderQueryResult {
						HasRefund = tradeState == "REFUND",
						OpenId = GetContent(xml, "openid"),
						Amount = GetValue<int>(xml, "total_fee") / 100m,
						TransactionId = GetContent(xml, "transaction_id"),
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatPayOrderQueryResult>(e.Message);
			};
		}

		public async Task<Result<WeChatPayRefundResult>> RefundAsync(string appId, string orderNo, string newRefundRequestNo, decimal totalOrderAmount, decimal refundAmount, string refundReason) {
			const string apiUrl = "https://api.mch.weixin.qq.com/secapi/pay/refund";

			var parameters = GetCommonParameters(appId);
			var more = new Dictionary<string, string> {
				{ "out_trade_no", orderNo },
				{ "out_refund_no", newRefundRequestNo },
				{ "total_fee", (totalOrderAmount * 100).ToString("f0") },
				{ "refund_fee", (refundAmount * 100).ToString("f0") },
				{ "refund_desc", refundReason },
			};
			foreach (var i in more) {
				parameters.Add(i.Key, i.Value);
			}

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters, useCert: true);

				if (!IsResultCodeSuccess(xml)) {
					return new Failure<WeChatPayRefundResult>(GetErrorDescription(xml));
				}
				return new Success<WeChatPayRefundResult> {
					Data = new WeChatPayRefundResult {
						RefundAmount = GetValue<int>(xml, "refund_fee") / 100m,
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatPayRefundResult>(e.Message);
			};
		}

		public async Task<Result<WeChatPayRefundQueryResult>> QueryRefundAsync(string appId, string refundRequestNo) {
			const string apiUrl = "https://api.mch.weixin.qq.com/pay/refundquery";

			var parameters = GetCommonParameters(appId);
			parameters.Add("out_refund_no", refundRequestNo);

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters);

				if (!IsResultCodeSuccess(xml) || (GetContent(xml, "refund_status_0") is not "SUCCESS" and not "PROCESSING")) {
					return new Failure<WeChatPayRefundQueryResult>(GetErrorDescription(xml));
				}
				return new Success<WeChatPayRefundQueryResult> {
					Data = new WeChatPayRefundQueryResult {
						RefundAmount = GetValue<int>(xml, "refund_fee_0") / 100m,
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatPayRefundQueryResult>(e.Message);
			};
		}

		public async Task<Result> CancelOrderAsync(string appId, string orderNo, bool allowToCancelAfterPaid = false) {
			const string apiUrl = "https://api.mch.weixin.qq.com/secapi/pay/reverse";

			if (!allowToCancelAfterPaid) {
				var queryResult = await QueryOrderAsync(appId, orderNo);
				if (queryResult.Ok) {
					return new Failure("订单已完成付款，未能撤销");
				}
			}

			var parameters = GetCommonParameters(appId);
			parameters.Add("out_trade_no", orderNo);

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters, useCert: true);
				if (IsResultCodeSuccess(xml)) {
					return new Success();
				}
				return new Failure(GetErrorDescription(xml));
			}
			catch (Exception e) {
				return new Failure(e.Message);
			};
		}

		public async Task<string> PostThenGetResultXmlAsync(string wechatApiUrl, Dictionary<string, string> apiParameters, bool useCert = false) {
			var sb = new StringBuilder();
			apiParameters.Add("sign_type", "MD5");

			//按Key排序，追加key=@MerchantSecret，合成FormData格式字符串
			var orderedNames = apiParameters.Keys.OrderBy(x => x).ToArray();
			foreach (var name in orderedNames) {
				sb.Append(name + "=" + apiParameters[name] + "&");
			}
			sb.Append("key=" + _options.MerchantSecret);

			//对格式后的字符串进行加密，获得sign，把sign和值加入原Dictionary
			var tobeSigned = sb.ToString();
			apiParameters.Add("sign", Crypto.MD5(tobeSigned));

			//再转换成XML
			sb.Clear();
			sb.Append("<xml>");
			foreach (var item in apiParameters) {
				sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
			}
			sb.Append("</xml>");
			var xml = sb.ToString();

			//将XML内容作为参数Post到api地址，返回的也是XML
			HttpResponseMessage? response = null;
			if (!useCert) {
				response = await DefaultHttpClient.Instance.PostAsync(wechatApiUrl, new StringContent(xml));
			}
			else {
				using var handler = !string.IsNullOrEmpty(_options.MerchantCertFile)
					? new WeChatPayCertifiedHttpClientHandler(_options.MerchantId, _options.MerchantCertFile)
					: new WeChatPayCertifiedHttpClientHandler(_options.MerchantId!);

				using var client = new HttpClient(handler);
				response = await client.PostAsync(wechatApiUrl, new StringContent(xml));
			}
			return await response.Content.ReadAsStringAsync();
		}

		public Result<WeChatPayNotifyResult> ParseNotifyResult(string xml) {
			try {
				if (!IsResultCodeSuccess(xml)) {
					return new Failure<WeChatPayNotifyResult>(GetErrorDescription(xml));
				}
				return new Success<WeChatPayNotifyResult> {
					Data = new WeChatPayNotifyResult {
						Amount = GetValue<decimal>(xml, "total_fee") / 100m,
						OpenId = GetContent(xml, "openid")!,
						OrderNo = GetContent(xml, "out_trade_no")!,
						TransactionId = GetContent(xml, "transaction_id")!,
						Attach = GetContent(xml, "attach"),
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WeChatPayNotifyResult>(e.Message);
			}
		}

		public string CreateNotifyRespondSuccessXml() {
			return "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
		}

		private Dictionary<string, string> GetCommonParameters(string wechatAppId) {
			return new Dictionary<string, string> {
				{ "appid", wechatAppId },
				{ "mch_id", _options.MerchantId! },
				{ "nonce_str", Crypto.RandomString(32) },
			};
		}


		private static string? GetContent(string fromXml, string nodeName) => fromXml.MidBy($"<{nodeName}><![CDATA[", $"]]></{nodeName}>");
		private static T GetValue<T>(string fromXml, string nodeName) where T : struct => fromXml.MidBy($"<{nodeName}>", $"</{nodeName}>").As<T>();
		private static bool IsResultCodeSuccess(string fromXml) => GetContent(fromXml, "result_code") == "SUCCESS";
		private static string? GetErrorDescription(string fromXml) => GetContent(fromXml, "err_code_des") ?? GetContent(fromXml, "trade_state_desc") ?? GetContent(fromXml, "return_msg");
	}
}
