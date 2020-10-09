//API document: https://pay.weixin.qq.com/wiki/doc/api/index.html

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Husky.WeChatIntegration.Models.Pay;

namespace Husky.WeChatIntegration.ServiceCategorized
{
	public class WeChatPayService
	{
		public WeChatPayService(WeChatAppConfig wechatConfig) {
			_wechatConfig = wechatConfig;
			_wechatConfig.RequireMerchantSettings();
		}

		private readonly WeChatAppConfig _wechatConfig;

		public WeChatPayJsApiParameter CreateJsApiPayParameter(string prepayId) {
			var nonceStr = Crypto.RandomString(32);
			var timeStamp = DateTime.Now.Timestamp();

			var sb = new StringBuilder();
			sb.Append("appId=" + _wechatConfig.MobilePlatformAppId);
			sb.Append("&nonceStr=" + nonceStr);
			sb.Append("&package=prepay_id=" + prepayId);
			sb.Append("&signType=MD5");
			sb.Append("&timeStamp=" + timeStamp);
			sb.Append("&key=" + _wechatConfig.MerchantSecret);
			var paySign = Crypto.MD5(sb.ToString()).ToUpper();

			return new WeChatPayJsApiParameter {
				timestamp = timeStamp,
				nonceStr = nonceStr,
				package = $"prepay_id={prepayId}",
				signType = "MD5",
				paySign = paySign
			};
		}

		public WeChatPayOrderModelUnifiedResult CreateUnifedOrder(WeChatPayOrderModel model) {
			var apiUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";

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
			foreach ( var i in more ) {
				parameters.Add(i.Key, i.Value);
			}
			if ( !model.AllowCreditCard ) {
				parameters.Add("limit_pay", "no_credit");
			}
			if ( !string.IsNullOrEmpty(model.Attach) ) {
				parameters.Add("attach", model.Attach);
			}

			try {
				var xml = PostThenGetResultXml(apiUrl, parameters);
				return new WeChatPayOrderModelUnifiedResult {
					Ok = IsOk(xml),
					Message = GetMessage(xml),
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

		public WeChatPayOrderQueryResult QueryOrder(string appId, string orderNo) {
			var apiUrl = "https://api.mch.weixin.qq.com/pay/orderquery";

			var parameters = GetCommonParameters(appId);
			parameters.Add("out_trade_no", orderNo);

			try {
				var xml = PostThenGetResultXml(apiUrl, parameters);
				var tradeState = GetCdata(xml, "trade_state");
				return new WeChatPayOrderQueryResult {
					Ok = tradeState == "REFUND" || tradeState == "SUCCESS",
					HasRefund = tradeState == "REFUND",
					Message = GetMessage(xml),
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

		public WeChatPayRefundResult Refund(string appId, string orderNo, string newRefundRequestNo, decimal totalOrderAmount, decimal refundAmount, string refundReason) {
			var apiUrl = "https://api.mch.weixin.qq.com/secapi/pay/refund";

			var parameters = GetCommonParameters(appId);
			var more = new Dictionary<string, string> {
				{ "out_trade_no", orderNo },
				{ "out_refund_no", newRefundRequestNo },
				{ "total_fee", (totalOrderAmount * 100).ToString("f0") },
				{ "refund_fee", (refundAmount * 100).ToString("f0") },
				{ "refund_desc", refundReason },
			};
			foreach ( var i in more ) {
				parameters.Add(i.Key, i.Value);
			}

			try {
				var xml = PostThenGetResultXml(apiUrl, parameters, true);
				return new WeChatPayRefundResult {
					Ok = IsOk(xml),
					Message = GetMessage(xml),
					RefundAmount = GetValue<int>(xml, "refund_fee") / 100m,
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

		public WeChatPayRefundQueryResult QueryRefund(string appId, string refundRequestNo) {
			var apiUrl = "https://api.mch.weixin.qq.com/pay/refundquery";

			var parameters = GetCommonParameters(appId);
			parameters.Add("out_refund_no", refundRequestNo);

			try {
				var xml = PostThenGetResultXml(apiUrl, parameters);
				return new WeChatPayRefundQueryResult {
					Ok = IsOk(xml) && (GetCdata(xml, "refund_status_0") == "SUCCESS" || GetCdata(xml, "refund_status_0") == "PROCESSING"),
					Message = GetMessage(xml),
					RefundAmount = GetValue<int>(xml, "refund_fee_0") / 100m,
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

		public string PostThenGetResultXml(string wechatApiUrl, Dictionary<string, string> apiParameters, bool useCert = false) {
			var sb = new StringBuilder();
			apiParameters.Add("sign_type", "MD5");

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
			using var webClient = useCert ? new CertifiedWebClient(_wechatConfig.MerchantId!) : new WebClient();

			var xmlResult = webClient.UploadString(wechatApiUrl, xml);
			return xmlResult;
		}

		public string CreateNotifyRespondSuccessXml() {
			return "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
		}

		public WeChatPayNotifyResult ParseNotifyResult(Stream stream) {
			try {
				var bytes = new byte[(int)stream.Length];
				stream.Read(bytes, 0, bytes.Length);

				var xml = Encoding.UTF8.GetString(bytes);
				return new WeChatPayNotifyResult {
					Ok = IsOk(xml),
					Message = GetMessage(xml),
					Amount = GetValue<decimal>(xml, "total_fee") / 100m,
					OpenId = GetCdata(xml, "openid")!,
					OrderNo = GetCdata(xml, "out_trade_no")!,
					TransactionId = GetCdata(xml, "transaction_id")!,
					Attach = GetCdata(xml, "attach"),
					OriginalResult = xml
				};
			}
			catch ( Exception e ) {
				return new WeChatPayNotifyResult {
					Ok = false,
					Message = e.Message
				};
			}
		}

		private Dictionary<string, string> GetCommonParameters(string wechatAppId) {
			return new Dictionary<string, string> {
				{ "appid", wechatAppId },
				{ "mch_id", _wechatConfig.MerchantId! },
				{ "nonce_str", Crypto.RandomString(32) },
			};
		}

		static string? GetCdata(string fromXml, string nodeName) => fromXml.MidBy($"<{nodeName}><![CDATA[", $"]]></{nodeName}>");
		static T GetValue<T>(string fromXml, string nodeName) where T : struct => fromXml.MidBy($"<{nodeName}>", $"</{nodeName}>").As<T>();
		static bool IsOk(string fromXml) => GetCdata(fromXml, "result_code") == "SUCCESS";
		static string? GetMessage(string fromXml) => GetCdata(fromXml, "err_code_des");
	}
}
