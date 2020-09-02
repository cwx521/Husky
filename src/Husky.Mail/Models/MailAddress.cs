using System;

namespace Husky.Mail
{
	public sealed partial class MailAddress
	{
		public string Name { get; set; }
		public string Address { get; set; }

		public override string ToString() {
			if ( string.IsNullOrWhiteSpace(Address) || string.IsNullOrWhiteSpace(Name) ) {
				return Address;
			}
			return $"{Name}<{Address}>";
		}
	}
}
