using Microsoft.EntityFrameworkCore;

namespace Husky.Mail.Data
{
	public interface IMailDbContext
	{
		DbContext Normalize();

		DbSet<MailSmtpProvider> MailSmtpProviders { get; set; }
		DbSet<MailRecord> MailRecords { get; set; }
		DbSet<MailRecordAttachment> MailRecordAttachments { get; set; }
	}
}
