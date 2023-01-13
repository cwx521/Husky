namespace Husky.KeyValues
{
	public static class KeyValueManagerExtensions
	{
		public static int HttpPostsMinimumIntervalMilliseconds(this IKeyValueManager keyValues, int fallback = 300) {
			return keyValues.GetOrAdd(nameof(HttpPostsMinimumIntervalMilliseconds), fallback);
		}
	}
}
