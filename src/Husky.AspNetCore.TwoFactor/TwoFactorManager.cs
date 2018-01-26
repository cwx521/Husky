using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.AspNetCore.AliyunSms;
using Husky.AspNetCore.Mail;
using Husky.AspNetCore.Principal;
using Husky.AspNetCore.TwoFactor.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore.TwoFactor
{
	public sealed partial class TwoFactorManager
	{
		public TwoFactorManager(IServiceProvider serviceProvider) {
			_db = serviceProvider.GetRequiredService<TwoFactorDbContext>();
			_config = serviceProvider.GetRequiredService<IConfiguration>();
			_my = serviceProvider.GetRequiredService<IPrincipalUser>();

			_mailSender = serviceProvider.GetRequiredService<IMailSender>();
			_aliyunSmsSender = serviceProvider.GetRequiredService<AliyunSmsSender>();
		}

		readonly TwoFactorDbContext _db;
		readonly IConfiguration _config;
		readonly IPrincipalUser _my;

		readonly IMailSender _mailSender;
		readonly AliyunSmsSender _aliyunSmsSender;

		public async Task<Result> RequestTwoFactorCode(string emailOrMobile, string messageTemplateArg0IsTheCode = null) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}

			var isEmail = emailOrMobile.IsEmail();
			var isMobile = emailOrMobile.IsMainlandMobile();

			if ( !isEmail && !isMobile ) {
				return new Failure($"无法发送到 '{emailOrMobile}' 。");
			}

			var code = new TwoFactorCode {
				UserIdString = _my.IdString,
				Code = new Random().Next(0, 1000000).ToString().PadLeft(6, '0'),
				SentTo = emailOrMobile
			};
			_db.Add(code);
			await _db.SaveChangesAsync();

			if ( isEmail ) {
				var template = messageTemplateArg0IsTheCode ?? string.Format("【{0}】动态验证码：{{0}}", _config.GetValue<string>("AppVariables:SiteName"));
				var content = string.Format(template, code.Code);
				await _mailSender.SendAsync("动态验证码", content, emailOrMobile);
			}
			else if ( isMobile ) {
				var smsConfig = _config.GetSection(AliyunSmsConfig.SectionName).Get<AliyunSmsConfig>();
				var smsArgument = new AliyunSmsArgument { code = code.Code };
				await _aliyunSmsSender.SendAsync(smsConfig, smsArgument, emailOrMobile);
			}
			return new Success();
		}

		public async Task<Result> VerifyTwoFactorCode(string emailOrMobile, string code, bool setIntoUsedAfterVerifying, int codeWithinMinutes = 20) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}
			if ( code == null ) {
				throw new ArgumentException(nameof(code));
			}

			var record = _db.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - codeWithinMinutes))
				.Where(x => x.SentTo == emailOrMobile)
				.Where(x => _my.IsAnonymous || x.UserIdString == _my.IdString)
				.OrderByDescending(x => x.Id)
				.Take(1)
				.FirstOrDefault();

			if ( record == null || string.Compare(code, record.Code, true) != 0 ) {
				return new Failure("验证码不正确或已经过期，请重试。");
			}
			if ( setIntoUsedAfterVerifying ) {
				record.IsUsed = true;
				await _db.SaveChangesAsync();
			}
			return new Success();
		}
	}
}