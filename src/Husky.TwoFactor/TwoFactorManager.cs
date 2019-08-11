using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Husky.AliyunSms;
using Husky.Principal;
using Husky.TwoFactor.Data;

namespace Husky.TwoFactor
{
	public sealed partial class TwoFactorManager
	{
		//IMailSender mailSender, 
		public TwoFactorManager(IPrincipalUser principal, TwoFactorDbContext twoFactorDb, AliyunSmsSender aliyunSmsSender) {
			_me = principal;
			_twoFactorDb = twoFactorDb;
			_aliyunSmsSender = aliyunSmsSender;
			//_mailSender = mailSender;
		}

		private readonly TwoFactorDbContext _twoFactorDb;
		private readonly IPrincipalUser _me;
		private readonly AliyunSmsSender _aliyunSmsSender;
		//private readonly IMailSender _mailSender;

		public async Task<Result> RequestTwoFactorCode(string emailOrMobile, string templateCode = null, string signName = null, string messageTemplateWithCodeAsArg0 = null) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}

			var isEmail = emailOrMobile.IsEmail();
			var isMobile = emailOrMobile.IsMainlandMobile();

			if ( !isEmail && !isMobile ) {
				return new Failure($"无法发送到 '{emailOrMobile}' ");
			}

			var code = new TwoFactorCode {
				UserIdString = _me.IdString,
				Code = new Random().Next(0, 1000000).ToString().PadLeft(6, '0'),
				SentTo = emailOrMobile
			};
			_twoFactorDb.Add(code);
			await _twoFactorDb.SaveChangesAsync();

			if ( isEmail ) {
				//var content = string.Format(messageTemplateWithCodeAsArg0, code.Code);
				//await _mailSender.SendAsync("动态验证码", content, emailOrMobile);
			}
			else if ( isMobile ) {
				var argument = new AliyunSmsArgument {
					SignName = signName,
					TemplateCode = templateCode,
					Parameters = new Dictionary<string, string> {
						{ "code", code.Code }
					}
				};
				await _aliyunSmsSender.SendAsync(argument, emailOrMobile);
			}
			return new Success();
		}

		public async Task<Result> VerifyTwoFactorCode(TwoFactorModel model, bool setIntoUsedAfterVerifying, int codeWithinMinutes = 15) {
			if ( model == null ) {
				throw new ArgumentNullException(nameof(model));
			}

			var record = _twoFactorDb.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - codeWithinMinutes))
				.Where(x => x.SentTo == model.SendTo)
				.Where(x => _me.IsAnonymous || x.UserIdString == _me.IdString)
				.OrderByDescending(x => x.Id)
				.Take(1)
				.FirstOrDefault();

			if ( record == null ) {
				return new Failure("验证码输入错误");
			}
			if ( record.ErrorTimes > 10 || string.Compare(model.Code, record.Code, true) != 0 ) {
				record.ErrorTimes++;
				await _twoFactorDb.SaveChangesAsync();
				return new Failure("验证码输入错误");
			}
			if ( setIntoUsedAfterVerifying ) {
				record.IsUsed = true;
				await _twoFactorDb.SaveChangesAsync();
			}
			return new Success();
		}
	}
}