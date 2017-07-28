using Husky.Data.ModelBuilding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Husky.MailTo.Data
{
	public class MailToDbContext : DbContext
	{
		public MailToDbContext(DbContextOptions options) : base(options) {
		}

		public DbSet<MailSmtpProvider> MailSmtpProviders { get; set; }
		public DbSet<MailRecord> MailRecords { get; set; }
		public DbSet<MailAttachment> MailAttachments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<MailRecord>().HasMany(x => x.Attachments).WithOne(x => x.Mail).HasForeignKey(x => x.MailId).IsRequired().OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Custom<MailToDbContext>();
		}
	}
}
