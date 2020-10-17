#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Mail.Data
{
	public static class MailDbModelBuilderHelper
	{
		public static void OnMailDbModelCreating(this ModelBuilder modelBuilder) {
			modelBuilder.Entity<MailSmtpProvider>()
				.HasMany(x => x.MailRecords)
				.WithOne(x => x.Smtp)
				.HasForeignKey(x => x.SmtpId);

			modelBuilder.Entity<MailRecord>()
				.HasMany(x => x.Attachments)
				.WithOne(x => x.Mail)
				.HasForeignKey(x => x.MailId);
		}
	}
}
