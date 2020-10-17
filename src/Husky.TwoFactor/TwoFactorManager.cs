using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Husky.AliyunSms;
using Husky.Mail;
using Husky.Principal;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.TwoFactor
{
	public sealed partial class TwoFactorManager
	{
		public TwoFactorManager(IPrincipalUser principal, ITwoFactorDbContext twoFactorDb, AliyunSmsSender? aliyunSmsSender, IMailSender? mailSender) {
			_me = principal;
			_twoFactorDb = twoFactorDb;
			_aliyunSmsSender = aliyunSmsSender;
			_mailSender = mailSender;
		}

		private readonly IPrincipalUser _me;
		private readonly ITwoFactorDbContext _twoFactorDb;
		private readonly AliyunSmsSender? _aliyunSmsSender;
		private readonly IMailSender? _mailSender;

		public async Task<Result> RequestCode(string mobileNumberOrEmailAddress, string? overrideAliyunSmsTemplateCode = null, string? overrideAliyunSmsSignName = null, string? messageTemplateWithCodeArg0 = null) {
			if ( mobileNumberOrEmailAddress == null ) {
				throw new ArgumentNullException(nameof(mobileNumberOrEmailAddress));
			}

			var isEmail = mobileNumberOrEmailAddress.IsEmail();
			var isMobile = mobileNumberOrEmailAddress.IsMainlandMobile();

			if ( !isEmail && !isMobile ) {
				return new Failure($"无法发送到 '{mobileNumberOrEmailAddress}' ");
			}

			if ( isMobile ) {
				var sentWithinMinute = _twoFactorDb.TwoFactorCodes
					.AsNoTracking()
					.Where(x => x.SentTo == mobileNumberOrEmailAddress)
					.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(-1))
					.Any();

				if ( sentWithinMinute ) {
					return new Failure("请求过于频繁");
				}
			}

			var code = new TwoFactorCode {
				UserId = _me.Id,
				Code = new Random().Next(0, 1000000).ToString("D6"),
				SentTo = mobileNumberOrEmailAddress
			};
			_twoFactorDb.TwoFactorCodes.Add(code);
			await _twoFactorDb.Normalize().SaveChangesAsync();

			if ( isEmail ) {
				if ( _mailSender == null ) {
					throw new Exception($"缺少邮件发送服务组件 {typeof(MailSender).Assembly.GetName()}，请在 ServiceCollection 中注入添加该服务");
				}
				var content = string.Format(messageTemplateWithCodeArg0 ?? "验证码：{0}", code.Code);
				await _mailSender.SendAsync("动态验证码", content, mobileNumberOrEmailAddress);
			}
			else if ( isMobile ) {
				if ( _aliyunSmsSender == null ) {
					throw new Exception($"缺少阿里云短讯发送服务组件 {typeof(AliyunSmsSender).Assembly.GetName()}，请在 ServiceCollection 中注入添加该服务");
				}
				var argument = new AliyunSmsArgument {
					SignName = overrideAliyunSmsSignName,
					TemplateCode = overrideAliyunSmsTemplateCode,
					Parameters = new Dictionary<string, string> {
						{ "code", code.Code }
					}
				};
				await _aliyunSmsSender.SendAsync(argument, mobileNumberOrEmailAddress);
			}

			return new Success();
		}

		public async Task<Result> RequestCodeThroughAliyunSms(string mobileNumber, string? overrideAliyunSmsTemplateCode = null, string? overrideAliyunSmsSignName = null) {
			return await RequestCode(mobileNumber, overrideAliyunSmsTemplateCode, overrideAliyunSmsSignName, null);
		}

		public async Task<Result> RequestCodeThroughEmail(string emailAddress, string? messageTemplateWithCodeArg0 = null) {
			return await RequestCode(emailAddress, null, null, messageTemplateWithCodeArg0);
		}

		public async Task<Result> VerifyCode(string sentTo, string code, bool setIntoUsedAfterVerifying, int withinMinutes = 15) {
			if ( sentTo == null ) {
				throw new ArgumentNullException(nameof(sentTo));
			}
			if ( code == null ) {
				throw new ArgumentNullException(nameof(code));
			}

			var record = _twoFactorDb.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - withinMinutes))
				.Where(x => x.SentTo == sentTo)
				.Where(x => _me.IsAnonymous || x.UserId == _me.Id)
				.OrderByDescending(x => x.Id)
				.FirstOrDefault();

			if ( record == null ) {
				return new Failure("验证码匹配失败");
			}
			if ( record.ErrorTimes > 10 || string.Compare(code, record.Code, true) != 0 ) {
				record.ErrorTimes++;
				await _twoFactorDb.Normalize().SaveChangesAsync();
				return new Failure("验证码输入错误");
			}
			if ( setIntoUsedAfterVerifying ) {
				record.IsUsed = true;
				await _twoFactorDb.Normalize().SaveChangesAsync();
			}
			return new Success();
		}

		public async Task<Result> VerifyCode(TwoFactorModel model, bool setIntoUsedAfterVerifying, int withinMinutes = 15) {
			if ( model == null ) {
				throw new ArgumentNullException(nameof(model));
			}
			return await VerifyCode(model.SendTo, model.Code, setIntoUsedAfterVerifying, withinMinutes);
		}
	}
}