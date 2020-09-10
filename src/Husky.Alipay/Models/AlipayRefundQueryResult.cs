using Alipay.AopSdk.Core.Response;

namespace Husky.Alipay.Models
{
	public class AlipayRefundQueryResult : Result
	{
		public string? RefundReason { get; internal set; }
		public decimal RefundAmount { get; internal set; }

		public AlipayTradeFastpayRefundQueryResponse? OriginalResult { get; internal set; }
	}
}
