namespace Husky.WeChatIntegration
{
	public class WxpayRefundModel
	{
		public string AppId { get; set; } = null!;
		public string TradeNo { get; set; } = null!;
		public string NewRefundRequestNo { get; set; } = null!;
		public decimal TotalPaidAmount { get; set; }
		public decimal RefundAmount { get; set; }
		public string? RefundReason { get; set; }
	}
}
