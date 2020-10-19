#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Mail.Data
{
	public class MailDbContext : DbContext, IMailDbContext
	{
		public MailDbContext(DbContextOptions<MailDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<MailSmtpProvider> MailSmtpProviders { get; set; }
		public DbSet<MailRecord> MailRecords { get; set; }
		public DbSet<MailRecordAttachment> MailRecordAttachments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyAdditionalCustomizedAnnotations();
			modelBuilder.OnMailDbModelCreating();
		}
	}
}
