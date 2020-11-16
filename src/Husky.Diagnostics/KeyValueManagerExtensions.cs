namespace Husky.KeyValues
{
	public static class KeyValueManagerExtensions
	{
		public static int LogRequestAsRepeatedIfSameWithinSeconds(this IKeyValueManager keyValues, int defaultValueIfNotExist = 60) {
			return keyValues.GetOrAdd(nameof(LogRequestAsRepeatedIfSameWithinSeconds), defaultValueIfNotExist);
		}
		
		public static int LogOperationAsRepeatedIfSameWithinSeconds(this IKeyValueManager keyValues, int defaultValueIfNotExist = 60) {
			return keyValues.GetOrAdd(nameof(LogOperationAsRepeatedIfSameWithinSeconds), defaultValueIfNotExist);
		}
	}
}
