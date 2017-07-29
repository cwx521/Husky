using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.MailTo.Abstractions
{
	public interface ISmtpProvider
	{
		string Host { get; set; }
		int Port { get; set; }
		string CredentialName { get; set; }
		string Password { get; set; }
	}
}
