namespace Husky.Principal.UserMessages
{
	public static class PrincipalUserExtensions
	{
		public static UserMessagesFunctions Messages(this IPrincipalUser principal) => new UserMessagesFunctions(principal);
	}
}
