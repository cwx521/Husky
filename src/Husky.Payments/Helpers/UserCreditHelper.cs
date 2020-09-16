using Husky.Payments.Data;

namespace Husky.Payments.Helpers
{
	public static class UserCreditHelper
	{
		public static void Calculate(this UserCredit credit, decimal balanceChange, bool doNegativeCheck = true) {
			var newValue = credit.Balance + balanceChange;
			if ( doNegativeCheck && newValue < 0 && balanceChange < 0 ) {
				throw new NegativeBalanceException();
			}
			credit.Balance = newValue;
		}
	}
}
