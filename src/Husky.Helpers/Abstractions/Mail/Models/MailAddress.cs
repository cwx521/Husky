using System;

namespace Husky.Mail
{
	public sealed partial class MailAddress
	{
		public string? Name { get; set; }
		public string Address { get; set; } = null!;

		public override string ToString() {
			if ( string.IsNullOrWhiteSpace(Name) ) {
				return Address;
			}
			return $"{Name}<{Address}>";
		}

		public static MailAddress Parse(string mailAddressString) {
			if ( mailAddressString == null ) {
				throw new ArgumentNullException(nameof(mailAddressString));
			}
			var ok = TryParse(mailAddressString, out var mailAddress);
			if ( !ok ) {
				throw new FormatException(
					$"'{mailAddressString}' is an invalid mail box address. " +
					$"A valid format should be like 'Your Name<youraccount@domain.com>'."
				);
			}
			return mailAddress!;
		}

		public static bool TryParse(string mailAddressString, out MailAddress? mailAddress) {
			if ( mailAddressString != null ) {
				if ( mailAddressString.IsEmail() ) {
					mailAddress = new MailAddress { Address = mailAddressString };
					return true;
				}

				var i = mailAddressString.IndexOf('<');
				var endedRight = mailAddressString.EndsWith(">");
				if ( i != -1 && endedRight ) {

					var name = mailAddressString[..i];
					var address = mailAddressString[(i + 1)..^1];

					if ( address.IsEmail() ) {
						mailAddress = new MailAddress { Name = name, Address = address };
						return true;
					}
				}
			}
			mailAddress = null;
			return false;
		}
	}
}
