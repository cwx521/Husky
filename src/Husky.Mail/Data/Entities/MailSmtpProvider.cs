using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Mail.Data
{
	public class MailSmtpProvider : ISmtpProvider
	{
		[Key]
		public Guid Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; } = Guid.NewGuid();

		[StringLength(100), Column(TypeName = "varchar(100)"), Required]
		public string Host { get; set; } = null!;

		public int Port { get; set; } = 25;

		public bool Ssl { get; set; }

		[StringLength(50), Column(TypeName = "varchar(50)"), Unique]
		public string? CredentialName { get; set; }

		[StringLength(64), Column(TypeName = "varchar(64)"), EditorBrowsable(EditorBrowsableState.Never)]
		public string? PasswordEncrypted { get; set; }

		[StringLength(50), Column(TypeName = "varchar(50)"), Required]
		public string SenderMailAddress { get; set; } = null!;

		[StringLength(50), Column(TypeName = "varchar(50)"), Required]
		public string SenderDisplayName { get; set; } = null!;

		public bool IsInUse { get; set; }


		[NotMapped]
		public string? Password {
			get => PasswordEncrypted == null ? null : Crypto.Decrypt(PasswordEncrypted, Id.ToString());
			set => PasswordEncrypted = value == null ? null : Crypto.Encrypt(value, Id.ToString());
		}


		// nav props

		public List<MailRecord> MailRecords { get; set; } = new List<MailRecord>();
	}
}