using Alipay.AopSdk.Core.Response;

namespace Husky.Alipay
{
	public class AlipayOrderQueryResult
	{
		public string? AlipayTradeNo { get; internal set; }
		public string? AlipayBuyerUserId { get; internal set; }
		public string? AlipayBuyerLogonId { get; internal set; }
		public decimal Amount { get; internal set; }

		public AlipayTradeQueryResponse? OriginalResult { get; internal set; }
	}
}
