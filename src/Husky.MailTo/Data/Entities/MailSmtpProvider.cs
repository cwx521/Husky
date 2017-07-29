using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.MailTo.Abstractions;
using Husky.Sugar;

namespace Husky.MailTo.Data
{
	public partial class MailSmtpProvider : ISmtpProvider
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Key, MaxLength(100), Column(Order = 0, TypeName = "varchar(100)")]
		public string Host { get; set; }

		[Key, MaxLength(50), Column(Order = 1, TypeName = "varchar(50)")]
		public string CredentialName { get; set; }

		public int Port { get; set; } = 25;

		[Required, MaxLength(64), Column(TypeName = "varchar(64)"), EditorBrowsable(EditorBrowsableState.Never)]
		public string PasswordEncrypted { get; set; }

		public bool IsInUse { get; set; }


		[NotMapped]
		public string Password {
			get { return Crypto.Decrypt(PasswordEncrypted, Id.ToString()); }
			set { Crypto.Encrypt(value, Id.ToString()); }
		}
	}
}