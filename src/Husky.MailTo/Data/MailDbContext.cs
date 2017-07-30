using Husky.Data;
using Husky.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Husky.MailTo.Data
{
	public class MailDbContext : DbContextBase
	{
		public MailDbContext(IDatabaseFinder finder) : base(finder) {
		}

		public DbSet<MailSmtpProvider> MailSmtpProviders { get; set; }
		public DbSet<MailRecord> MailRecords { get; set; }
		public DbSet<MailRecordAttachment> MailRecordAttachments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<MailSmtpProvider>(smtp => {
				smtp.HasAlternateKey(x => new { x.Host, x.CredentialName });
				smtp.HasMany(x => x.SentMails).WithOne(x => x.Smtp).HasForeignKey(x => x.SmtpId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
			});
			modelBuilder.Entity<MailRecord>().HasMany(x => x.Attachments).WithOne(x => x.Mail).HasForeignKey(x => x.MailId).IsRequired().OnDelete(DeleteBehavior.Cascade);
			
			base.OnModelCreating(modelBuilder);
		}
	}
}
