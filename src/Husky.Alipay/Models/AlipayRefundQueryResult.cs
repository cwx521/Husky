using Aop.Api.Response;

namespace Husky.Alipay
{
	public record AlipayRefundQueryResult
	{
		public string? RefundReason { get; internal init; }
		public decimal RefundAmount { get; internal init; }

		public AlipayTradeFastpayRefundQueryResponse? OriginalResult { get; internal init; }
	}
}
