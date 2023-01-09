using Aop.Api.Response;

namespace Husky.Alipay
{
	public record AlipayOrderQueryResult
	{
		public decimal Amount { get; internal init; }
		public string? AlipayTradeNo { get; internal init; }
		public string? AlipayBuyerId { get; internal init; }

		public AlipayTradeQueryResponse? OriginalResult { get; internal init; }
	}
}
