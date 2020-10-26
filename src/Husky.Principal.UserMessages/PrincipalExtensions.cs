namespace Husky.Principal.UserMessages
{
	public static partial class PrincipalExtensions
	{
		public static UserMessagesManager Messages(this IPrincipalUser principal) => new UserMessagesManager(principal);
	}
}
