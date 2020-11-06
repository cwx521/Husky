using Aop.Api.Response;

namespace Husky.Alipay
{
	public class AlipayRefundQueryResult
	{
		public string? RefundReason { get; internal set; }
		public decimal RefundAmount { get; internal set; }

		public AlipayTradeFastpayRefundQueryResponse? OriginalResult { get; internal set; }
	}
}
