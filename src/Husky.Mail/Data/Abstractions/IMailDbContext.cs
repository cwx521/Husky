using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.Mail.Data
{
	public interface IMailDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		DbSet<MailSmtpProvider> MailSmtpProviders { get; set; }
		DbSet<MailRecord> MailRecords { get; set; }
		DbSet<MailRecordAttachment> MailRecordAttachments { get; set; }
	}
}
