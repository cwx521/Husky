using Aop.Api.Response;

namespace Husky.Alipay
{
	public record AlipayOrderF2FPayResult
	{
		public decimal Amount { get; internal init; }
		public string? AlipayTradeNo { get; internal init; }
		public string? AlipayBuyerId { get; internal init; }
		public bool AwaitPaying { get; internal set; }

		public AlipayTradePayResponse? OriginalResult { get; internal init; }
	}
}
