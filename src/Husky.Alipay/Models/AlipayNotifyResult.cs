using System.Collections.Generic;

namespace Husky.Alipay
{
	public record AlipayNotifyResult
	{
		public decimal Amount { get; internal init; }
		public string? OrderNo { get; internal init; }
		public string? AlipayTradeNo { get; internal init; }
		public string? AlipayBuyerId { get; internal init; }
		public Dictionary<string, string>? OriginalResult { get; internal init; }
	}
}
