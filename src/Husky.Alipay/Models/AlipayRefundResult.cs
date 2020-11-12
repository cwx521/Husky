using Aop.Api.Response;

namespace Husky.Alipay
{
	public record AlipayRefundResult
	{
		public decimal RefundAmount { get; internal init; }
		public decimal AggregatedRefundAmount { get; internal init; }

		public AlipayTradeRefundResponse? OriginalResult { get; internal init; }
	}
}
