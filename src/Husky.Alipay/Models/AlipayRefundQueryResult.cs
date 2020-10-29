using Alipay.AopSdk.Core.Response;

namespace Husky.Alipay.Models
{
	public class AlipayRefundQueryResult
	{
		public string? RefundReason { get; internal set; }
		public decimal RefundAmount { get; internal set; }

		public AlipayTradeFastpayRefundQueryResponse? OriginalResult { get; internal set; }
	}
}
