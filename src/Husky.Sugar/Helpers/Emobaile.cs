namespace Husky.Sugar
{
	public class Emobaile
	{
		public Emobaile(string emailOrMobile) {
			AccountName = emailOrMobile;
			IsEmail = AccountName.IsEmail();
			IsMobile = AccountName.IsMainlandMobile();
		}

		public string AccountName { get; }
		public bool IsEmail { get; }
		public bool IsMobile { get; }

		public bool IsValid => IsEmail || IsMobile;

		public EmobaileType? Type {
			get {
				if ( IsEmail ) return EmobaileType.Email;
				if ( IsMobile ) return EmobaileType.Mobile;
				return null;
			}
		}
	}
}
