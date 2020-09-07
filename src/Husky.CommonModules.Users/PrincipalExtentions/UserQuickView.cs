using System;

namespace Husky.Principal
{
	public class UserQuickView
	{
		public string? PhoneNumber { get; set; }
		public string? PhotoUrl { get; set; }
		public DateTime RegisteredTime { get; internal set; }

		public ActionAwait AwaitChangePassword { get; set; }
	}
}
