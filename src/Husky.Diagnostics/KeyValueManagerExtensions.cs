namespace Husky.KeyValues
{
	public static class KeyValueManagerExtensions
	{
		public static int LogRequestAsRepeatedIfSameWithinSeconds(this IKeyValueManager keyValues, int fallback = 60) {
			return keyValues.GetOrAdd(nameof(LogRequestAsRepeatedIfSameWithinSeconds), fallback);
		}
		
		public static int LogOperationAsRepeatedIfSameWithinSeconds(this IKeyValueManager keyValues, int fallback = 60) {
			return keyValues.GetOrAdd(nameof(LogOperationAsRepeatedIfSameWithinSeconds), fallback);
		}
	}
}
