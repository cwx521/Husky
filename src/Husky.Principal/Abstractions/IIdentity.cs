namespace Husky.Principal
{
	public interface IIdentity
	{
		int Id { get; set; }
		string DisplayName { get; set; }
		bool IsConsolidated { get; set; }

		bool IsAnonymous { get; }
		bool IsAuthenticated { get; }
	}
}