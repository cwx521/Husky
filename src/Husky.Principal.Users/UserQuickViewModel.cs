﻿using System;

namespace Husky.Principal.Users
{
	public class UserQuickViewModel
	{
		public string? PhoneNumber { get; set; }
		public string? EmailAddress { get; set; }
		public string? PhotoUrl { get; set; }
		public Todo AwaitChangePassword { get; set; }
		public DateTime RegisteredTime { get; internal init; }
	}
}
