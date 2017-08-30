using System;
using Husky.Sugar;

namespace Husky.Mail
{
	public sealed class MailAddress
	{
		public string Name { get; set; }
		public string Address { get; set; }

		public static MailAddress Parse(string mailAddressString) {
			if ( mailAddressString == null ) {
				throw new ArgumentNullException(nameof(mailAddressString));
			}
			if ( mailAddressString.IsEmail() ) {
				return new MailAddress { Address = mailAddressString };
			}
			if ( mailAddressString.IndexOf('<') != -1 && mailAddressString.EndsWith(">") ) {
				var name = mailAddressString.Left("<", true).Trim();
				var address = mailAddressString.Right("<", true).TrimEnd('>');
				if ( address.IsEmail() ) {
					return new MailAddress { Name = name, Address = address };
				}
			}
			throw new FormatException($"'{mailAddressString}' is an invalid mail box address format. Valid format should be like 'Your Name<youraccount@domain.com>'.");
		}

		public static bool TryParse(string mailAddressString, out MailAddress mailAddress) {
			if ( mailAddressString != null ) {
				if ( mailAddressString.IsEmail() ) {
					mailAddress = new MailAddress { Address = mailAddressString };
					return true;
				}
				if ( mailAddressString.IndexOf('<') != -1 && mailAddressString.EndsWith(">") ) {
					var name = mailAddressString.Left("<", true);
					var address = mailAddressString.Right("<", true).TrimEnd('>');
					if ( address.IsEmail() ) {
						mailAddress = new MailAddress { Name = name, Address = address };
						return true;
					}
				}
			}
			mailAddress = null;
			return false;
		}

		public override string ToString() {
			if ( string.IsNullOrWhiteSpace(Address) || string.IsNullOrWhiteSpace(Name) ) {
				return Address;
			}
			return $"{Name}<{Address}>";
		}
	}
}
