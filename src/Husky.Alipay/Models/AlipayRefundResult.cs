using Aop.Api.Response;

namespace Husky.Alipay
{
	public class AlipayRefundResult
	{
		public decimal RefundAmount { get; internal set; }
		public decimal AggregatedRefundAmount { get; internal set; }

		public AlipayTradeRefundResponse? OriginalResult { get; internal set; }
	}
}
