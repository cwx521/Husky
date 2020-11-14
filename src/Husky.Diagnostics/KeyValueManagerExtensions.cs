namespace Husky.KeyValues
{
	public static class KeyValueManagerExtensions
	{
		public static int LogAsRepeatedIfVisitAgainWithinSeconds(this IKeyValueManager keyValues, int defaultValueIfNotExist = 60) {
			return keyValues.GetOrAdd(nameof(LogAsRepeatedIfVisitAgainWithinSeconds), defaultValueIfNotExist);
		}

		public static int LogAsRepeatedIfOperateAgainWithinSeconds(this IKeyValueManager keyValues, int defaultValueIfNotExist = 60) {
			return keyValues.GetOrAdd(nameof(LogAsRepeatedIfOperateAgainWithinSeconds), defaultValueIfNotExist);
		}
	}
}
