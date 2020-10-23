namespace Husky.TwoFactor
{
	public class TwoFactorModel : ITwoFactorModel
	{
		public virtual string SendTo { get; set; } = null!;
		public virtual string Code { get; set; } = null!;
	}
}
