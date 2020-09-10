using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Husky.WeChatIntegration.Models.Pay;
using Microsoft.AspNetCore.Http;

namespace Husky.WeChatIntegration
{
	public class WeChatPayService
	{
		public WeChatPayService(IHttpContextAccessor http, WeChatAppConfig wechatConfig) {
			_http = http.HttpContext;
			_wechatConfig = wechatConfig;

			_wechatConfig.RequireMerchantSettings();
		}

		private readonly HttpContext _http;
		private readonly WeChatAppConfig _wechatConfig;

		public WeChatJsApiPayParameter CreateJsApiPayParameter(string prepayId) {
			var nonceStr = Crypto.RandomString(32);
			var timeStamp = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).Ticks / 1000;

			var sb = new StringBuilder();
			sb.Append("appId=" + _wechatConfig.MobilePlatformAppId);
			sb.Append("&nonceStr=" + nonceStr);
			sb.Append("&package=prepay_id=" + prepayId);
			sb.Append("&signType=MD5");
			sb.Append("&timeStamp=" + timeStamp);
			sb.Append("&key=" + _wechatConfig.MerchantSecret);
			var paySign = Crypto.MD5(sb.ToString()).ToUpper();

			return new WeChatJsApiPayParameter {
				timestamp = timeStamp,
				nonceStr = nonceStr,
				package = $"prepay_id={prepayId}",
				signType = "MD5",
				paySign = paySign
			};
		}

		public WeChatPayOrderModelUnifiedResult CreateUnifedOrder(WeChatPayOrderModel model) {
			var apiUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";

			var parameters = GetCommonParameters(model.AppId);
			var more = new Dictionary<string, string> {
				{ "time_start", DateTime.Now.ToString("yyyyMMddHHmmss") },
				{ "time_expire", DateTime.Now.Add(model.Expiration).ToString("yyyyMMddHHmmss") },
				{ "spbill_create_ip", _http.Connection.RemoteIpAddress.ToString() },
				{ "device_info", model.Device },
				{ "trade_type", model.TradeType.ToString() },
				{ "out_trade_no", model.OrderId },
				{ "notify_url", model.NotifyUrl },
				{ "openid", model.OpenId },
				{ "total_fee", (model.Amount * 100).ToString("f0") },
				{ "body", model.Body },
				{ "attach", model.Attach ?? Crypto.MD5(model.OrderId + model.Amount) },
			};
			foreach ( var i in more ) {
				parameters.Add(i.Key, i.Value);
			}
			if ( !model.AllowCreditCard ) {
				parameters.Add("limit_pay", "no_credit");
			}

			try {
				var xml = PostThenGetResultXml(apiUrl, parameters);
				return new WeChatPayOrderModelUnifiedResult {
					Ok = true,
					Message = GetCdata(xml, "return_msg"),
					PrepayId = GetCdata(xml, "prepay_id"),
					CodeUrl = GetCdata(xml, "code_url"),
					OriginalResult = xml
				};
			}
			catch ( Exception e ) {
				return new WeChatPayOrderModelUnifiedResult {
					Ok = false,
					Message = e.Message
				};
			};
		}

		public WeChatPayOrderQueryResult QueryOrder(string appId, string orderId) {
			var apiUrl = "https://api.mch.weixin.qq.com/pay/orderquery";

			var parameters = GetCommonParameters(appId);
			parameters.Add("out_trade_no", orderId);

			try {
				var xml = PostThenGetResultXml(apiUrl, parameters);
				return new WeChatPayOrderQueryResult {
					Ok = GetCdata(xml, "trade_state") == "SUCCESS",
					Message = GetCdata(xml, "return_msg"),
					OpenId = GetCdata(xml, "openid"),
					Amount = GetValue<int>(xml, "total_fee") / 100m,
					TransactionId = GetCdata(xml, "transaction_id"),
					OriginalResult = xml
				};
			}
			catch ( Exception e ) {
				return new WeChatPayOrderQueryResult {
					Ok = false,
					Message = e.Message
				};
			};
		}

		public WeChatPayRefundResult Refund(string appId, string orderId, string newRefundRequestOrderId, decimal totalOrderAmount, decimal refundAmount) {
			var apiUrl = "https://api.mch.weixin.qq.com/secapi/pay/refund";

			var parameters = GetCommonParameters(appId);
			var more = new Dictionary<string, string> {
				{ "out_trade_no", orderId },
				{ "out_refund_no", newRefundRequestOrderId },
				{ "total_fee", (totalOrderAmount * 100).ToString("f0") },
				{ "refund_fee", (refundAmount * 100).ToString("f0") },
			};
			foreach ( var i in more ) {
				parameters.Add(i.Key, i.Value);
			}

			try {
				var xml = PostThenGetResultXml(apiUrl, parameters);
				return new WeChatPayRefundResult {
					Ok = GetCdata(xml, "result_code") == "SUCCESS",
					Message = GetCdata(xml, "return_msg"),
					AggregatedRefundAmount = GetValue<int>(xml, "refund_fee") / 100,
					OriginalResult = xml
				};
			}
			catch ( Exception e ) {
				return new WeChatPayRefundResult {
					Ok = false,
					Message = e.Message
				};
			};
		}

		public WeChatPayRefundQueryResult QueryRefund(string appId, string refundRequestOrderId) {
			var apiUrl = "https://api.mch.weixin.qq.com/pay/refundquery";

			var parameters = GetCommonParameters(appId);
			parameters.Add("out_refund_no", refundRequestOrderId);

			try {
				var xml = PostThenGetResultXml(apiUrl, parameters);
				return new WeChatPayRefundQueryResult {
					Ok = GetCdata(xml, "result_code") == "SUCCESS" &&
						(GetCdata(xml, "refund_status_0") == "SUCCESS" || GetCdata(xml, "refund_status_0") == "PROCESSING"),
					Message = GetCdata(xml, "return_msg"),
					RefundAmount = GetValue<int>(xml, "refund_fee_0") / 100,
					OriginalResult = xml
				};
			}
			catch ( Exception e ) {
				return new WeChatPayRefundQueryResult {
					Ok = false,
					Message = e.Message
				};
			};
		}

		public string PostThenGetResultXml(string wechatApiUrl, Dictionary<string, string> apiParameters) {
			var sb = new StringBuilder();

			//按Key排序，追加key=@MerchantSecret，合成FormData格式字符串
			var orderedNames = apiParameters.Keys.OrderBy(x => x).ToArray();
			foreach ( var name in orderedNames ) {
				sb.Append(name + "=" + apiParameters[name] + "&");
			}
			sb.Append("key=" + _wechatConfig.MerchantSecret);

			//对格式后的字符串进行加密，获得sign，把sign和值加入原Dictionary
			var tobeSigned = sb.ToString();
			apiParameters.Add("sign", Crypto.MD5(tobeSigned).ToUpper());

			//再转换成XML
			sb.Clear();
			sb.Append("<xml>");
			foreach ( var item in apiParameters ) {
				sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
			}
			sb.Append("</xml>");
			var xml = sb.ToString();

			//将XML内容作为参数Post到api地址，返回的也是XML
			using var webClient = new WebClient();
			return webClient.UploadString(wechatApiUrl, xml);
		}

		private Dictionary<string, string> GetCommonParameters(string wechatAppId) {
			return new Dictionary<string, string> {
				{ "appid", wechatAppId },
				{ "mch_id", _wechatConfig.MerchantId! },
				{ "nonce_str", Crypto.RandomString(32) },
			};
		}

		static T GetValue<T>(string fromXml, string nodeName) where T : struct => fromXml.MidBy($"<{nodeName}>", $"</{nodeName}>").As<T>();
		static string? GetCdata(string fromXml, string nodeName) => fromXml.MidBy($"<{nodeName}><![CDATA[", $"]]></{nodeName}>");
	}
}
