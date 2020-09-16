#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Husky.EF;
using Microsoft.EntityFrameworkCore;

namespace Husky.Payments.Data
{
	public partial class PaymentsDbContext : DbContext
	{
		public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) {
		}

		public DbSet<CreditType> CreditTypes { get; set; }

		public DbSet<BalanceChange> BalanceChanges { get; set; }
		public DbSet<BalanceChangeEvent> BalanceChangeEvents { get; set; }

		public DbSet<PayDeposit> Deposits { get; set; }
		public DbSet<PayRefund> PayRefunds { get; set; }
		public DbSet<PayWithdrawal> PayWithdrawals { get; set; }

		public DbSet<UserCredit> UserCredits { get; set; }


		protected override void OnModelCreating(ModelBuilder mb) {
			mb.ApplyHuskyAnnotations();
			mb.OnPaymentsModelCreating();
		}
	}
}
