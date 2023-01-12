using Aop.Api.Response;

namespace Husky.Alipay
{
	public record AlipayTradeQueryResult
	{
		public decimal Amount { get; internal init; }
		public string? AlipayTradeNo { get; internal init; }
		public string? AlipayBuyerId { get; internal init; }

		public AlipayTradeQueryResponse? OriginalResult { get; internal init; }
	}
}
