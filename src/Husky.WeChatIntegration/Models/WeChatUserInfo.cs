﻿namespace Husky.WeChatIntegration
{
	public class WeChatUserInfo
	{
		public string OpenId { get; set; }
		public string UnionId { get; set; }
		public string NickName { get; set; }
		public Sex Sex { get; set; }
		public string Province { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string HeadImageUrl { get; set; }
	}
}