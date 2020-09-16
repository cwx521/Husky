#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Payments.Data
{
	public static class PaymentsModelBuilderHelper
	{
		public static void OnPaymentsModelCreating(this ModelBuilder mb) {

			//UserCredit
			mb.Entity<UserCredit>(credit => {
				credit.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
			});

			//BalanceChange
			mb.Entity<BalanceChange>(bc => {
				bc.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
				bc.HasOne(x => x.CreditType).WithMany().HasForeignKey(x => x.CreditTypeId);
				bc.HasOne(x => x.Event).WithMany(x => x.BalanceChanges).HasForeignKey(x => x.EventId);
			});

			//Deposit,Refund,Withdrawal
			mb.Entity<PayDeposit>(deposit => {
				deposit.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
				deposit.HasOne(x => x.CreditType).WithMany().HasForeignKey(x => x.CreditTypeId);
			});
			mb.Entity<PayRefund>(refund => {
				refund.HasOne(x => x.Deposit).WithMany(x => x.Refunds).HasForeignKey(x => x.DepositId);
				refund.HasOne(x => x.CreditType).WithMany().HasForeignKey(x => x.CreditTypeId);
			});
			mb.Entity<PayWithdrawal>(withdrawal => {
				withdrawal.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
				withdrawal.HasOne(x => x.CreditType).WithMany().HasForeignKey(x => x.CreditTypeId);
			});
		}
	}
}
