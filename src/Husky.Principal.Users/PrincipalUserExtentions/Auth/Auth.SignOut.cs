namespace Husky.Principal.Users
{
	public partial class UserAuthFunctions
	{
		public void SignOut() {
			if ( _me.IsAuthenticated ) {
				_me.AbandonCache();
				_me.IdentityManager?.DeleteIdentity();
				_me.Id = 0;
				_me.IsConsolidated = false;
			}
		}
	}
}
