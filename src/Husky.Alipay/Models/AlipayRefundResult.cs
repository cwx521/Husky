using Alipay.AopSdk.Core.Response;

namespace Husky.Alipay.Models
{
	public class AlipayRefundResult
	{
		public decimal RefundAmount { get; internal set; }
		public decimal AggregatedRefundAmount { get; internal set; }

		public AlipayTradeRefundResponse? OriginalResult { get; internal set; }
	}
}
