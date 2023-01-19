//API document: https://pay.weixin.qq.com/wiki/doc/api/index.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Husky.WeChatIntegration.Models.Pay;
using Microsoft.AspNetCore.Http;

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

		public WxpayJsApiParameter CreateJsApiPayParameter(string prepayId) {
			var nonceStr = Crypto.RandomString(32);
			var timestamp = DateTime.Now.Timestamp();

			var sb = new StringBuilder();
			sb.Append("appId=" + _options.MobilePlatformAppId);
			sb.Append("&nonceStr=" + nonceStr);
			sb.Append("&package=prepay_id=" + prepayId);
			sb.Append("&signType=MD5");
			sb.Append("&timeStamp=" + timestamp);
			sb.Append("&key=" + _options.MerchantSecret);
			var paySign = Crypto.MD5(sb.ToString()).ToUpper();

			return new WxpayJsApiParameter {
				timestamp = timestamp,
				nonceStr = nonceStr,
				package = $"prepay_id={prepayId}",
				signType = "MD5",
				paySign = paySign
			};
		}

		public async Task<Result<WxpayTradeCreationResult>> CreateUnifiedOrderAsync(WxpayTradeCreationModel model) {
			const string apiUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";

			var now = DateTime.Now;
			var parameters = GetCommonParams(model.AppId);
			var more = new Dictionary<string, string> {
				{ "time_start", now.ToString("yyyyMMddHHmmss") },
				{ "time_expire", now.Add(model.Expiration).ToString("yyyyMMddHHmmss") },
				{ "device_info", model.Device },
				{ "trade_type", model.TradeType.ToUpper() },
				{ "spbill_create_ip", model.IPAddress },
				{ "out_trade_no", model.TradeNo },
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

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters);

				if (!IsResultCodeSuccess(xml)) {
					return new Failure<WxpayTradeCreationResult>(GetResultDescription(xml));
				}
				return new Success<WxpayTradeCreationResult> {
					Data = new WxpayTradeCreationResult {
						PrepayId = GetValue(xml, "prepay_id"),
						CodeUrl = GetValue(xml, "code_url"),
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WxpayTradeCreationResult>(e.Message);
			};
		}

		public async Task<Result<WxpayTradeMicroPayResult>> MicroPay(WxpayTradeMicroPayModel model) {
			const string apiUrl = "https://api.mch.weixin.qq.com/pay/micropay";

			var now = DateTime.Now;
			var parameters = GetCommonParams(model.AppId);
			var more = new Dictionary<string, string> {
				{ "time_start", now.ToString("yyyyMMddHHmmss") },
				{ "time_expire", now.Add(model.Expiration).ToString("yyyyMMddHHmmss") },
				{ "spbill_create_ip", model.IPAddress },
				{ "out_trade_no", model.TradeNo },
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

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters);

				return new Result<WxpayTradeMicroPayResult> {
					Ok = IsResultCodeSuccess(xml) && GetValue(xml, "trade_type") == "MICROPAY",
					Message = GetResultDescription(xml),
					Data = new WxpayTradeMicroPayResult {
						OpenId = GetValue(xml, "openid"),
						TransactionId = GetValue(xml, "transaction_id"),
						Amount = GetValue<int>(xml, "total_fee") / 100m,
						AwaitPaying = GetValue(xml, "err_code") == "USERPAYING",
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WxpayTradeMicroPayResult>(e.Message);
			};
		}

		public async Task<Result<WxpayTradeQueryResult>> QueryTradeAsync(string appId, string tradeNo) {
			const string apiUrl = "https://api.mch.weixin.qq.com/pay/orderquery";

			var parameters = GetCommonParams(appId);
			parameters.Add("out_trade_no", tradeNo);

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters);
				var tradeState = GetValue(xml, "trade_state");

				if (tradeState != "REFUND" && tradeState != "SUCCESS") {
					return new Failure<WxpayTradeQueryResult>(GetResultDescription(xml));
				}

				return new Success<WxpayTradeQueryResult> {
					Data = new WxpayTradeQueryResult {
						HasRefund = tradeState == "REFUND",
						OpenId = GetValue(xml, "openid"),
						Amount = GetValue<int>(xml, "total_fee") / 100m,
						TransactionId = GetValue(xml, "transaction_id"),
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WxpayTradeQueryResult>(e.Message);
			};
		}

		public async Task<Result<WxpayRefundResult>> RefundAsync(WxpayRefundModel model) {
			const string apiUrl = "https://api.mch.weixin.qq.com/secapi/pay/refund";

			var parameters = GetCommonParams(model.AppId);
			var more = new Dictionary<string, string> {
				{ "out_trade_no", model.TradeNo },
				{ "out_refund_no", model.NewRefundRequestNo },
				{ "total_fee", (model.TotalPaidAmount * 100).ToString("f0") },
				{ "refund_fee", (model.RefundAmount * 100).ToString("f0") },
				{ "refund_desc", model.RefundReason ?? string.Empty },
			};
			foreach (var i in more) {
				parameters.Add(i.Key, i.Value);
			}

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters, useCert: true);

				if (!IsResultCodeSuccess(xml)) {
					return new Failure<WxpayRefundResult>(GetResultDescription(xml));
				}
				return new Success<WxpayRefundResult> {
					Data = new WxpayRefundResult {
						RefundAmount = GetValue<int>(xml, "refund_fee") / 100m,
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WxpayRefundResult>(e.Message);
			};
		}

		public async Task<Result<WxpayRefundQueryResult>> QueryRefundAsync(string appId, string refundRequestNo) {
			const string apiUrl = "https://api.mch.weixin.qq.com/pay/refundquery";

			var parameters = GetCommonParams(appId);
			parameters.Add("out_refund_no", refundRequestNo);

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters);

				if (!IsResultCodeSuccess(xml) || (GetValue(xml, "refund_status_0") is not "SUCCESS" and not "PROCESSING")) {
					return new Failure<WxpayRefundQueryResult>(GetResultDescription(xml));
				}
				return new Success<WxpayRefundQueryResult> {
					Data = new WxpayRefundQueryResult {
						RefundAmount = GetValue<int>(xml, "refund_fee_0") / 100m,
						OriginalResult = xml
					}
				};
			}
			catch (Exception e) {
				return new Failure<WxpayRefundQueryResult>(e.Message);
			};
		}

		public async Task<Result> CancelTradeAsync(string appId, string tradeNo, bool allowToCancelAfterPaid = false) {
			const string apiUrl = "https://api.mch.weixin.qq.com/secapi/pay/reverse";

			if (!allowToCancelAfterPaid) {
				var queryResult = await QueryTradeAsync(appId, tradeNo);
				if (queryResult.Ok) {
					return new Failure("订单已完成付款，未能撤销");
				}
			}

			var parameters = GetCommonParams(appId);
			parameters.Add("out_trade_no", tradeNo);

			try {
				var xml = await PostThenGetResultXmlAsync(apiUrl, parameters, useCert: true);
				return new Result(IsResultCodeSuccess(xml), GetResultDescription(xml));
			}
			catch (Exception e) {
				return new Failure(e.Message);
			};
		}

		public async Task<string> PostThenGetResultXmlAsync(string wechatApiUrl, Dictionary<string, string> apiParams, bool useCert = false) {
			apiParams.Add("sign_type", "MD5");

			//按Key排序，追加key=@MerchantSecret，合成FormData格式字符串
			var sortedNames = apiParams.Keys.OrderBy(x => x).ToArray();
			var sb = new StringBuilder();
			foreach (var name in sortedNames) {
				sb.Append(name + "=" + apiParams[name] + "&");
			}
			sb.Append("key=" + _options.MerchantSecret);

			//对格式后的字符串进行加密，获得sign，把sign和值加入原Dictionary
			var toBeSigned = sb.ToString();
			apiParams.Add("sign", Crypto.MD5(toBeSigned));

			//再转换成XML
			sb.Clear();
			sb.Append("<xml>");
			foreach (var item in apiParams) {
				sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
			}
			sb.Append("</xml>");
			var xml = sb.ToString();

			//将XML内容作为参数Post到api地址，返回的也是XML
			HttpResponseMessage? response = null;
			if (!useCert) {
				response = await HttpClientSingleton.Instance.PostAsync(wechatApiUrl, new StringContent(xml));
			}
			else {
				using var handler = !string.IsNullOrEmpty(_options.MerchantCertFile)
					? new CertifiedWxpayHttpClientHandler(_options.MerchantId, _options.MerchantCertFile)
					: new CertifiedWxpayHttpClientHandler(_options.MerchantId!);

				using var client = new HttpClient(handler);
				response = await client.PostAsync(wechatApiUrl, new StringContent(xml));
			}
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<Result<WxpayNotifyResult>> ParseNotifyResult(HttpRequest request) {
			var bytes = new byte[(int)request.ContentLength!];
			await request.Body.ReadAsync(bytes.AsMemory(0, bytes.Length));
			var xml = Encoding.UTF8.GetString(bytes);
			return ParseNotifyResult(xml);
		}

		public Result<WxpayNotifyResult> ParseNotifyResult(string xml) {
			if (!IsResultCodeSuccess(xml)) {
				return new Failure<WxpayNotifyResult>(GetResultDescription(xml));
			}

			var dict = new Dictionary<string, string>();
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);

			foreach (XmlNode node in xmlDoc.ChildNodes) {
				if (node.Name != "sign") {
					var value = node.Value ?? node.InnerText;
					if (!string.IsNullOrEmpty(value)) {
						dict.Add(node.Name, value);
					}
				}
			}
			var sb = new StringBuilder();
			var sortedNames = dict.Keys.OrderBy(x => x).ToArray();
			foreach (var name in sortedNames) {
				sb.Append(name + "=" + dict[name] + "&");
			}
			sb.Append("key=" + _options.MerchantSecret);

			var toBeSigned = sb.ToString();
			var sign = Crypto.MD5(toBeSigned).ToUpper();
			if (sign != dict["sign"]) {
				return new Failure<WxpayNotifyResult>("验证签名失败");
			}

			return new Success<WxpayNotifyResult> {
				Data = new WxpayNotifyResult {
					Amount = GetValue<decimal>(xml, "total_fee") / 100m,
					OpenId = GetValue(xml, "openid")!,
					TradeNo = GetValue(xml, "out_trade_no")!,
					TransactionId = GetValue(xml, "transaction_id")!,
					OriginalResult = xml
				}
			};
		}

		public string CreateNotifyRespondSuccessXml() {
			return "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
		}

		private Dictionary<string, string> GetCommonParams(string wechatAppId) {
			return new Dictionary<string, string> {
				{ "appid", wechatAppId },
				{ "mch_id", _options.MerchantId! },
				{ "nonce_str", Crypto.RandomString(32) },
			};
		}


		private static string? GetValue(string fromXml, string nodeName) => fromXml.MidBy($"<{nodeName}><![CDATA[", $"]]></{nodeName}>");
		private static T GetValue<T>(string fromXml, string nodeName) where T : struct => fromXml.MidBy($"<{nodeName}>", $"</{nodeName}>").As<T>();
		private static bool IsResultCodeSuccess(string fromXml) => GetValue(fromXml, "result_code") == "SUCCESS";
		private static string? GetResultDescription(string fromXml) => GetValue(fromXml, "err_code_des") ?? GetValue(fromXml, "trade_state_desc") ?? GetValue(fromXml, "return_msg");
	}
}
