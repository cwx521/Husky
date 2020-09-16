using System;

namespace Husky.Payments
{
	public sealed class NegativeBalanceException : Exception
	{
		public NegativeBalanceException(string message = "Balance will be negative after calculation.") : base(message) {
		}
	}
}
