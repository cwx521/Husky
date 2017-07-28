namespace Husky.Smtp.Abstractions
{
	public interface ISmtpProvider
	{
		string Host { get; }
		int Port { get; }
		string CredentialName { get; }
		string Password { get; }
		string DisplayMailAddress { get; }
		string DisplayName { get; }
	}
}