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
	}
}
