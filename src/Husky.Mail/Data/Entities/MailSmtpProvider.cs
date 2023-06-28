using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Husky.Mail.Data
{
	[Index(nameof(CredentialName), IsUnique = true)]
	public class MailSmtpProvider : ISmtpProvider
	{
		[Key]
		public Guid Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; } = Guid.NewGuid();

		[StringLength(100), Unicode(false), Required]
		public string Host { get; set; } = null!;

		public int Port { get; set; } = 25;

		public bool Ssl { get; set; }

		[StringLength(50), Unicode(false)]
		public string? CredentialName { get; set; }

		[StringLength(64), Unicode(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string? PasswordEncrypted { get; set; }

		[StringLength(50), Unicode(false), Required]
		public string SenderMailAddress { get; set; } = null!;

		[StringLength(50), Unicode(false), Required]
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