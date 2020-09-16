using Husky.Principal.SessionData;

namespace Husky.Principal
{
	partial class AuthManager
	{
		public void SignOut() {
			if ( _me.IsAuthenticated ) {
				_me.AbandonSessionData();
				_me.IdentityManager.DeleteIdentity();
				_me.Id = 0;
			}
		}
	}
}
