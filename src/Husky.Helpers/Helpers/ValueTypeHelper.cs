using System.Linq;

namespace Husky.Syntactic.Natural
{
	public static class ValueTypeHelper
	{
		public static bool Is<T>(this T self, params T[] inRange) => inRange.Contains(self);
		public static bool IsNot<T>(this T self, params T[] shouldBeOutOf) => !shouldBeOutOf.Contains(self);
	}
}
