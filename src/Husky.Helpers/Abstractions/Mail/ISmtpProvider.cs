namespace Husky.Mail
{
	public interface ISmtpProvider
	{
		string Host { get; }
		int Port { get; }
		bool Ssl { get; }
		string SenderMailAddress { get; }
		string SenderDisplayName { get; }
		string? CredentialName { get; }
		string? Password { get; }
	}
}