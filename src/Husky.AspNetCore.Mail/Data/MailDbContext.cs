using Microsoft.EntityFrameworkCore;

namespace Husky.AspNetCore.Mail.Data
{
	public class MailDbContext : DbContext
	{
		public MailDbContext(DbContextOptions<MailDbContext> options) : base(options) {
		}

		public DbSet<MailSmtpProvider> MailSmtpProviders { get; set; }
		public DbSet<MailRecord> MailRecords { get; set; }
		public DbSet<MailRecordAttachment> MailRecordAttachments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<MailSmtpProvider>()
				.HasMany(x => x.SentMails)
				.WithOne(x => x.Smtp)
				.HasForeignKey(x => x.SmtpId)
				.IsRequired(false);

			modelBuilder.Entity<MailRecord>()
				.HasMany(x => x.Attachments)
				.WithOne(x => x.Mail)
				.HasForeignKey(x => x.MailId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
