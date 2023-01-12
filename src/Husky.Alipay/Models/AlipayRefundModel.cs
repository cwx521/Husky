﻿namespace Husky.Alipay
{
	public class AlipayRefundModel
	{
		public decimal RefundAmount { get; set; }
		public string OriginalTradeNo { get; set; } = null!;
		public string RefundRequestNo { get; set; } = null!;
		public string? RefundReason { get; set; }
	}
}
