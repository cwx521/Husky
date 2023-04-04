using System;
using System.Collections.Generic;
using System.Linq;

namespace Husky
{
	public static class OrderIdGen
	{
		public static string New() {
			SetNow();
			var str = string.Concat(Year, Month, Day, Hour, Time);
			return str + Validate(str);
		}

		public static string NewLiteral() {
			SetNow();
			var str = string.Concat(_now.ToString("yyyyMMddHH"), Time);
			return str + Validate(str);
		}

		private static DateTime _now;
		private static long _lastOne = DateTime.UtcNow.Ticks / 10000;

		public static bool TryParse(string? str, out DateTime utcDateTime) {
			if ( str == null || (str.Length != 12 && str.Length != 18) || Validate(str[..^1]) != str.Last() - '0' ) {
				utcDateTime = DateTime.MinValue;
				return false;
			}
			var makeup = str.Length == 12
				? string.Concat(
						str[0] - 'A' + offsetYear, '-',                             //yyyy-
						str[1] - 'A' + 1, '-',                                      //MM-
						str[2] <= '9' ? (str[2] - '0') : (str[2] - 'A' + 10),       //dd
						' ',
						'Z' - str[3], ':',                                          //H:
						str[4..6], ':',												//mm:
						str[8..10]												    //ss
					)
				: string.Concat(
						str[0..4], '-',                                             //yyyy-
						str[4..6], '-',                                             //MM-
						str[6..8],                                                  //dd
						' ',
						str[8..10], ":",                                            //HH:
						str[10..12], ':',                                           //mm:
						str[14..16]											        //ss
					);
			return DateTime.TryParse(makeup, out utcDateTime);
		}
		public static bool IsValid(string? str) => TryParse(str, out _);

		//use 1 char to present Year, 0=2020, 1=2021, 2=2022 ...
		private static readonly int offsetYear = 2020;
		private static char Year => (char)('A' + (_now.Year - offsetYear));

		//use 1 char to present Month, A=Jan, B=Feb, C=Mar ...
		private static char Month => (char)('A' + _now.Month - 1);

		//use 1 char to present Day, 1=1, 2=2, ... A=10, B=11 ...
		private static char Day => (char)(_now.Day + (_now.Day < 10 ? '0' : ('A' - 10)));

		//use 1 char to present Hour, Z=0, Y=1, X=2 ...
		private static char Hour => (char)('Z' - _now.Hour);

		//use 7 chars to present Minutes and Seconds, the ordering is obfuscated
		private static string Time => new(TimePart().ToArray());

		//append a validation digit number, which takes one char, from 0-9
		private static int Validate(string str) => str.Aggregate(0, (result, i) => result + i) * 97 % 10;

		private static IEnumerable<char> TimePart() {
			var str = _now.ToString("mmssfff");
			var obfuscate = new[] { 1, 2, 6, 5, 3, 4, 7 };
			foreach ( var i in obfuscate ) {
				yield return str[i - 1];
			}
		}

		private static void SetNow() {
			_now = DateTime.UtcNow;
			if ( _now.Ticks / 10000 <= _lastOne ) {
				_lastOne++;
				_now = new DateTime(_lastOne * 10000);
			}
			else {
				_lastOne = _now.Ticks / 10000;
			}
		}
	}
}