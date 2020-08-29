using Microsoft.EntityFrameworkCore;

namespace Husky.Mail.Data
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
