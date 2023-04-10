using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.Mail.Data
{
	public interface IMailDbContext : IDbContext, IDisposable, IAsyncDisposable
	{
		public DbSet<MailSmtpProvider> MailSmtpProviders { get; set; }
		public DbSet<MailRecord> MailRecords { get; set; }
		public DbSet<MailRecordAttachment> MailRecordAttachments { get; set; }
	}
}
