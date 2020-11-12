using Aop.Api.Response;

namespace Husky.Alipay
{
	public record AlipayOrderQueryResult
	{
		public string? AlipayTradeNo { get; internal init; }
		public string? AlipayBuyerUserId { get; internal init; }
		public string? AlipayBuyerLogonId { get; internal init; }
		public decimal Amount { get; internal init; }

		public AlipayTradeQueryResponse? OriginalResult { get; internal init; }
	}
}
