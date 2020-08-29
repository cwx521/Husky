using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Husky.Syntactic.Natural;

namespace Husky
{
	public static class OrderIdGen
	{
		public static string New() {
			var str = string.Concat(Hour, Month, Day, Time, Year);
			return str + Validation(str);
		}

		public static bool TryParse(string str, out DateTime datetime) {
			if ( str == null || str.Length != 12 || Validation(str.Substring(0, 11)) != str[11] - '0' ) {
				datetime = DateTime.MinValue;
				return false;
			}
			var makeup = string.Concat(
				str[10] - '0' + 2011, '-',
				str[1] - 'A' + 1, '-',
				str[2] <= '9' ? str[2] - '0' : str[2] - 'A' + 10, ' ',
				'Z' - str[0], ':',
				string.Concat(str[5], str[7]), ':',
				string.Concat(str[9], str[4])
			);
			return DateTime.TryParse(makeup, out datetime);
		}
		public static bool IsValid(string orderId) => TryParse(orderId, out _);

		private static char Year => (char)('0' + (DateTime.Now.Year - 2011));
		private static char Month => (char)('A' + DateTime.Now.Month - 1);
		private static char Day => (char)(DateTime.Now.Day + (DateTime.Now.Day < 10 ? '0' : 'A' - 10));
		private static char Hour => (char)('Z' - DateTime.Now.Hour);
		private static string Time => string.Join("", TimeString());
		private static int Validation(string str) => str.Aggregate(0, (result, i) => result + i) * 9 % 10;

		private static int _antiDup = 0;
		private static IEnumerable<char> TimeString() {
			var x = DateTime.Now.ToString("mmssfff").AsInt();
			if ( x <= _antiDup ) {
				x = _antiDup + 1;
			}
			_antiDup = x;
			var str = x.ToString("D7");
			var ordering = new[] { 7, 4, 1, 5, 2, 6, 3 };
			foreach ( var i in ordering ) {
				yield return str[i - 1];
			}
		}
	}
}