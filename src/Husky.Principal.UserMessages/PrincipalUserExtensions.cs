namespace Husky.Principal.UserMessages
{
	public static partial class PrincipalUserExtensions
	{
		public static UserMessagesFunctions Messages(this IPrincipalUser principal) => new UserMessagesFunctions(principal);
	}
}
