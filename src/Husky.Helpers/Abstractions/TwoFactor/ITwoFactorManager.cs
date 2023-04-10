using System.Threading.Tasks;

namespace Husky.TwoFactor
{
	public interface ITwoFactorManager
	{
		Task<Result> SendCodeAsync(
			string cellphoneOrEmail,
			string? overrideContentTemplateWithArg0 = null,
			string? overrideTemplateCode = null,
			string? overrideSignName = null
		);

		Task<Result> SendCodeThroughSmsAsync(
			string cellphone,
			string? overrideContentTemplateWithArg0 = null,
			string? overrideTemplateCode = null,
			string? overrideSignName = null
		);

		Task<Result> SendCodeThroughEmailAsync(
			string emailAddress,
			string? overrideContentTemplateWithArg0 = null,
			string? overrideSignName = null
		);

		Task<Result> VerifyCodeAsync(
			string sentTo,
			string code,
			bool setIntoUsedWhenSuccess,
			int codeExpirationMinutes = 15
		);
	}
}