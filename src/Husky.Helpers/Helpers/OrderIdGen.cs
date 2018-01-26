using System;
using System.Collections.Generic;
using System.Linq;

namespace Husky
{
	public static class OrderIdGen
	{
		public static string New() {
			var str = string.Concat(Hour, Month, Day, Time, Salt, Year);
			return str + Validation(str);
		}

		public static bool TryParse(string str, out DateTime datetime) {
			if ( str == null || str.Length != 15 || Validation(str.Substring(0, 14)) != str[14] - '0' ) {
				datetime = DateTime.MinValue;
				return false;
			}
			var makeup = string.Concat(
				str[13] - '0' + 2011, '-',
				str[1] - 'A' + 1, '-',
				str[2] <= '9' ? str[2] - '0' : str[2] - 'A' + 10, ' ',
				'Z' - str[0], ':',
				string.Concat(str[5], str[7]), ':',
				string.Concat(str[9], str[4])
			);
			return DateTime.TryParse(makeup, out datetime);
		}

		public static bool IsValid(string orderId) => TryParse(orderId, out var dt);


		static int _seed;

		static string Salt => new Random(Crypto.RandomNumber()).Next(0, 1000).ToString("D3");
		static char Year => (char)('0' + (DateTime.Now.Year - 2011));
		static char Month => (char)('A' + DateTime.Now.Month - 1);
		static char Day => (char)(DateTime.Now.Day + (DateTime.Now.Day < 10 ? '0' : 'A' - 10));
		static char Hour => (char)('Z' - DateTime.Now.Hour);
		static string Time => string.Join("", _Time());

		static IEnumerable<char> _Time() {
			var add = _seed++ >= 1000 ? (_seed = 0) : _seed;
			var str = (DateTime.Now.ToString("mmssfff").AsInt() + add).ToString("D7");
			var ordering = new[] { 7, 4, 1, 5, 2, 6, 3 };
			foreach ( var i in ordering ) yield return str[i - 1];
		}

		static int Validation(string str) => str.Aggregate(0, (result, i) => result + i) * 9 % 10;
	}
}