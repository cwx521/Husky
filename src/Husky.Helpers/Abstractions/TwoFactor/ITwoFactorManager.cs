using System.Threading.Tasks;

namespace Husky.TwoFactor
{
	public interface ITwoFactorManager
	{
		Task<Result> SendCodeThroughSmsAsync(string mobileNumber,
			string? givenCode,
			string? overrideMessageTemplateWithCodeArg0 = null,
			string? overrideSmsTemplateAlias = null,
			string? overrideSmsSignName = null);

		Task<Result> SendCodeThroughEmailAsync(string emailAddress,
			string? givenCode,
			string? messageTemplateWithCodeArg0 = null,
			string? overrideSmsSignName = null);

		Task<Result> VerifyCodeAsync(string sentTo, 
			string code, 
			bool setIntoUsedIfSuccess, 
			int codeUsableWithinMinutes = 15);
	}
}