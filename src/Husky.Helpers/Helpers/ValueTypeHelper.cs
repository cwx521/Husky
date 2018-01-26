using System.Linq;

namespace Husky.Syntactic.Natural
{
	public static class ValueTypeHelper
	{
		public static bool Is<T>(this T self, params T[] inRange) where T : struct {
			return inRange.Contains(self);
		}

		public static bool IsNot<T>(this T self, params T[] rangeShouldBeOutOf) where T : struct {
			return !rangeShouldBeOutOf.Contains(self);
		}
	}
}
