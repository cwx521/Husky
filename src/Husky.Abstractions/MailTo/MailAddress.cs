using System;
using Husky.Sugar;

namespace Husky.MailTo
{
	public sealed class MailAddress
	{
		public string Name { get; set; }
		public string Address { get; set; }

		public static MailAddress Parse(string formattedString) {
			if ( formattedString == null ) {
				throw new ArgumentNullException(nameof(formattedString));
			}
			if ( formattedString.IsEmail() ) {
				return new MailAddress { Address = formattedString };
			}
			if ( formattedString.IndexOf('<') != -1 && formattedString.EndsWith(">") ) {
				var name = formattedString.Left("<", true).Trim();
				var address = formattedString.Right("<", true).TrimEnd('>');
				if ( address.IsEmail() ) {
					return new MailAddress { Name = name, Address = address };
				}
			}
			throw new FormatException($"'{formattedString}' is an invalid mail box address format. Valid format should be like 'Your Name<youraccount@domain.com>'.");
		}

		public static bool TryParse(string formattedString, out MailAddress mailAddress) {
			if ( formattedString != null ) {
				if ( formattedString.IsEmail() ) {
					mailAddress = new MailAddress { Address = formattedString };
					return true;
				}
				if ( formattedString.IndexOf('<') != -1 && formattedString.EndsWith(">") ) {
					var name = formattedString.Left("<", true);
					var address = formattedString.Right("<", true).TrimEnd('>');
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
