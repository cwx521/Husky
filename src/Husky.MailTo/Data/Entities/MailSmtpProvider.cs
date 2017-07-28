using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.Sugar;

namespace Husky.MailTo.Data
{
	public partial class MailSmtpProvider
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required, MaxLength(80), Column(TypeName = "varchar")]
		public string Host { get; set; }

		public int Port { get; set; } = 25;

		[Required, MaxLength(50), Column(TypeName = "varchar")]
		public string CredentialName { get; set; }

		[Required, MaxLength(64), Column(TypeName = "varchar"), EditorBrowsable(EditorBrowsableState.Never)]
		public string PasswordEncrypted { get; set; }

		[Required, MaxLength(50), Column(TypeName = "varchar")]
		public string DisplayMailAddress { get; set; }

		[Required, MaxLength(50)]
		public string DisplayName { get; set; }

		public bool IsInUse { get; set; }


		[NotMapped]
		public string Password {
			get { return Crypto.Decrypt(PasswordEncrypted, Id.ToString()); }
			set { Crypto.Encrypt(value, Id.ToString()); }
		}
	}
}