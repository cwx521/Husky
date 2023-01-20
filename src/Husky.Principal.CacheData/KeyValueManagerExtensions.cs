namespace Husky.KeyValues
{
	public static class KeyValueManagerExtensions
	{
		public static int PrincipalCacheDataBagWillExpireAfterSeconds(this IKeyValueManager keyValues, int fallback = 1800) {
			return keyValues.GetOrAdd(nameof(PrincipalCacheDataBagWillExpireAfterSeconds), fallback);
		}
	}
}
