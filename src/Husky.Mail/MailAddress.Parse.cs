﻿using System;

namespace Husky.Mail
{
	partial class MailAddress
	{
		public static MailAddress Parse(string mailAddressString) {
			if ( mailAddressString == null ) {
				throw new ArgumentNullException(nameof(mailAddressString));
			}
			var ok = TryParse(mailAddressString, out var mailAddress);
			if ( !ok ) {
				throw new FormatException($"'{mailAddressString}' is an invalid mail box address. A valid format should be like 'Your Name<youraccount@domain.com>'.");
			}
			return mailAddress;
		}

		public static bool TryParse(string mailAddressString, out MailAddress mailAddress) {
			if ( mailAddressString != null ) {
				if ( mailAddressString.IsEmail() ) {
					mailAddress = new MailAddress { Address = mailAddressString };
					return true;
				}
				if ( mailAddressString.IndexOf('<') != -1 && mailAddressString.EndsWith(">") ) {
					var name = mailAddressString.LeftBy("<", true);
					var address = mailAddressString.RightBy("<", true).TrimEnd('>');
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