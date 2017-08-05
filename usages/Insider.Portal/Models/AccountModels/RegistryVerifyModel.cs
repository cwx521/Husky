using System;
using System.ComponentModel.DataAnnotations;
using Husky.TwoFactor.Data;
using Insider.Portal.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Insider.Portal.Models.AccountModels
{
	public class RegistryVerifyModel
	{
		[Display(Name = "帐号")]
		public string AccountName { get; set; }

		[Required(ErrorMessage = "请输入您接收到的验证码。")]
		[RegularExpression(@"^\d{6}$", ErrorMessage = "请输入正确的验证码数字，长度6位。")]
		[Remote(nameof(ApiController.IsTwoFactorCodeValid), "Api", AdditionalFields = nameof(AccountName) + "," + nameof(TwoFactorPurpose), HttpMethod = "POST", ErrorMessage = "验证码不正确。")]
		[Display(Name = "验证码")]
		public string TwoFactorCode { get; set; }

		public bool AutoSend { get; set; }

		public TwoFactorPurpose TwoFactorPurpose => TwoFactorPurpose.ExistenceCheck;
	}
}
