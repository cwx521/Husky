using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Mail.Data
{
	public partial class MailSmtpProvider : ISmtpProvider
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required, MaxLength(100), Column(TypeName = "varchar(100)")]
		public string Host { get; set; }

		[MaxLength(50), Column(TypeName = "varchar(50)")]
		public string CredentialName { get; set; }

		public int Port { get; set; } = 25;

		public bool Ssl { get; set; }

		[MaxLength(64), Column(TypeName = "varchar(64)"), EditorBrowsable(EditorBrowsableState.Never)]
		public string PasswordEncrypted { get; set; }

		[MaxLength(50), Column(TypeName = "varchar(50)")]
		public string SenderMailAddress { get; set; }

		[MaxLength(50), Column(TypeName = "varchar(50)")]
		public string SenderDisplayName { get; set; }

		public bool IsInUse { get; set; }


		[NotMapped]
		public string Password {
			get => Crypto.Decrypt(PasswordEncrypted, Id.ToString());
			set => PasswordEncrypted = Crypto.Decrypt(value, Id.ToString());
		}


		// nav props

		public List<MailRecord> MailRecords { get; set; } = new List<MailRecord>();
	}
}