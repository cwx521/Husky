namespace Husky.TwoFactor
{
	public interface ITwoFactorModel
	{
		string SendTo { get; set; }
		string Code { get; set; }
	}
}
