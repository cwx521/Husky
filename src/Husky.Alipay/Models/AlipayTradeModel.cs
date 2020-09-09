using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Alipay
{
	public class AlipayTradeModel
	{
		public decimal Amount { get; set; }
		public string InternalOrderNo { get; set; } = null!;
		public string Subject { get; set; } = null!;
		public string? Body { get; set; }

		public string CallbackUrl { get; set; } = null!;
		public string NotifyUrl { get; set; } = null!;
		public bool OnMobileDevice { get; set; }
	}
}
