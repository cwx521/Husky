using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Newtonsoft.Json;

namespace Husky.Alipay
{
	public static class AlipayServiceHelper
	{
		public static string GenerateAlipayPaymentUrl(this AlipayService alipay, AlipayTradeModel tradeModel) {
			var payModel = new AlipayTradePayModel {
				Subject = tradeModel.Subject,
				Body = tradeModel.Body,
				OutTradeNo = tradeModel.InternalOrderNo,
				TotalAmount = tradeModel.Amount.ToString("f2"),
				ProductCode = "FAST_INSTANT_TRADE_PAY"
			};

			if ( tradeModel.OnMobileDevice ) {
				var request = new AlipayTradeWapPayRequest();
				request.SetReturnUrl(tradeModel.CallbackUrl);
				request.SetNotifyUrl(tradeModel.NotifyUrl);
				request.SetBizModel(payModel);

				var response = alipay.SdkExecute(request);
				return alipay.Options.Gatewayurl + "?" + response.Body;
			}
			else {
				var request = new AlipayTradePagePayRequest();
				request.SetReturnUrl(tradeModel.CallbackUrl);
				request.SetNotifyUrl(tradeModel.NotifyUrl);
				request.SetBizModel(payModel);

				var response = alipay.SdkExecute(request);
				return alipay.Options.Gatewayurl + "?" + response.Body;
			}
		}

		public static AlipayOrderQueryResult? QueryOrder(this AlipayService alipay, string internalOrderNo) {
			var model = new AlipayTradeQueryModel { OutTradeNo = internalOrderNo };
			var request = new AlipayTradeQueryRequest();
			request.SetBizModel(model);

			try {
				var response = alipay.Execute(request);
				var json = response.Body;
				dynamic obj = JsonConvert.DeserializeObject<dynamic>(json).alipay_trade_query_response;

				//Sample of obj
				//{{
				//  "code": "10000",
				//  "msg": "Success",
				//  "buyer_logon_id": "177******21",
				//  "buyer_pay_amount": "0.00",
				//  "buyer_user_id": "2088802603431380",
				//  "invoice_amount": "0.00",
				//  "out_trade_no": "CI9780578202",
				//  "point_amount": "0.00",
				//  "receipt_amount": "0.00",
				//  "send_pay_date": "2020-09-09 23:07:41",
				//  "total_amount": "0.10",
				//  "trade_no": "2020090922001431380573310660",
				//  "trade_status": "TRADE_SUCCESS"
				//}}

				if ( obj.trade_status != "TRADE_SUCCESS" || obj.msg != "Success" ) {
					return null;
				}

				return new AlipayOrderQueryResult {
					AlipayTradeNo = (string)obj.trade_no,
					AlipayBuyerLogonId = (string)obj.buyer_logon_id,
					TotalAmount = decimal.Parse((string)obj.total_amount)
				};
			}
			catch { return null; }
		}

		public static Result Refund(this AlipayService alipay, string originalInternalOrderId, string refundRequestOrderId, decimal refundAmount, string refundReason) {
			var model = new AlipayTradeRefundModel {
				OutTradeNo = originalInternalOrderId,
				OutRequestNo = refundRequestOrderId,
				RefundAmount = refundAmount.ToString("f2"),
				RefundReason = refundReason
			};

			var request = new AlipayTradeRefundRequest();
			request.SetBizModel(model);
			try {
				var response = alipay.Execute(request);
				var json = response.Body;
				dynamic obj = JsonConvert.DeserializeObject<dynamic>(json).alipay_trade_refund_response;

				//Sample of obj
				//{{
				//  "code": "10000",
				//  "msg": "Success",
				//  "buyer_logon_id": "177******21",
				//  "buyer_user_id": "2088802603431380",
				//  "fund_change": "Y",
				//  "gmt_refund_pay": "2020-09-09 23:10:49",
				//  "out_trade_no": "CI9321000201",
				//  "refund_fee": "0.01",
				//  "send_back_fee": "0.00",
				//  "trade_no": "2020090922001431380570383224"
				//}}

				var ok = obj.msg == "Success";
				var message = (string)obj.sub_msg;
				return new Result(ok, message);
			}
			catch {
				return new Failure("请求接口未能成功");
			}
		}


		public static AlipayRefundQueryResult? QueryRefund(this AlipayService alipay, string originalInternalOrderId, string refundRequestOrderId) {
			var model = new AlipayTradeFastpayRefundQueryModel {
				OutTradeNo = originalInternalOrderId,
				OutRequestNo = refundRequestOrderId
			};
			var request = new AlipayTradeFastpayRefundQueryRequest();
			request.SetBizModel(model);

			try {
				var response = alipay.Execute(request);
				var json = response.Body;
				dynamic obj = JsonConvert.DeserializeObject<dynamic>(json).alipay_trade_fastpay_refund_query_response;

				//Sample of obj
				//{{
				//  "code": "10000",
				//  "msg": "Success",
				//  "out_request_no": "ZIA490857500",
				//  "out_trade_no": "ZIA050059405",
				//  "refund_amount": "0.01",
				//  "total_amount": "0.10",
				//  "trade_no": "2020091022001431380570927130"
				//}}

				if ( obj.msg != "Success" ) {
					return null;
				}

				return new AlipayRefundQueryResult {
					AlipayTradeNo = (string)obj.trade_no,
					RefundAmount = decimal.Parse((string)obj.refund_amount),
					TotalAmount = decimal.Parse((string)obj.total_amount)
				};
			}
			catch {
				return null;
			}
		}
	}
}
