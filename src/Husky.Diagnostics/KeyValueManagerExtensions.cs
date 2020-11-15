namespace Husky.KeyValues
{
	public static class KeyValueManagerExtensions
	{
		public static int LogRequestAsRepeatedIfVisitAgainWithinSeconds(this IKeyValueManager keyValues, int defaultValueIfNotExist = 60) {
			return keyValues.GetOrAdd(nameof(LogRequestAsRepeatedIfVisitAgainWithinSeconds), defaultValueIfNotExist);
		}

		public static int LogOperationAsRepeatedIfOperateAgainWithinSeconds(this IKeyValueManager keyValues, int defaultValueIfNotExist = 60) {
			return keyValues.GetOrAdd(nameof(LogOperationAsRepeatedIfOperateAgainWithinSeconds), defaultValueIfNotExist);
		}
	}
}
