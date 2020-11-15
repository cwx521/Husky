namespace Husky.KeyValues
{
	public static class KeyValueManagerExtensions
	{
		public static int PrincipalCacheDataBagWillExpireAfterSeconds(this IKeyValueManager keyValues, int defaultValueIfNotExist = 1800) {
			return keyValues.GetOrAdd(nameof(PrincipalCacheDataBagWillExpireAfterSeconds), defaultValueIfNotExist);
		}
	}
}
