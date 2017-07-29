using Husky.Data;
using Husky.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Husky.MailTo.Data
{
	public class MailDbContext : DbContextBase
	{
		public MailDbContext(IDatabaseFinder connstr) : base(connstr) {
		}

		public DbSet<MailSmtpProvider> MailSmtpProviders { get; set; }
		public DbSet<MailRecord> MailRecords { get; set; }
		public DbSet<MailRecordAttachment> MailRecordAttachments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<MailRecord>().HasMany(x => x.Attachments).WithOne(x => x.Mail).HasForeignKey(x => x.MailId).IsRequired().OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<MailSmtpProvider>().HasKey(x => new { x.Host, x.CredentialName });
			modelBuilder.Entity<MailSmtpProvider>().HasAlternateKey(x => x.Id);
			base.OnModelCreating(modelBuilder);
		}
	}
}
