﻿namespace Husky.Mail
{
	public interface ISmtpProvider
	{
		string Host { get; }
		int Port { get; }
		bool Ssl { get; }
		string CredentialName { get; }
		string Password { get; }
		string SenderMailAddress { get; }
		string SenderDisplayName { get; }
	}
}