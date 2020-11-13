using System;
using System.Collections.Generic;
using System.Linq;

namespace Husky
{
	public static class OrderIdGen
	{
		public static string New() {
			var str = string.Concat(Year, Month, Day, Hour, Time);
			return str + Validation(str);
		}

		public static bool TryParse(string? str, out DateTime datetime) {
			if ( str == null || str.Length != 12 || Validation(str[0..11]) != str[11] - '0' ) {
				datetime = DateTime.MinValue;
				return false;
			}
			var makeup = string.Concat(
				str[0] - 'A' + offsetYear, '-',                             //yyyy-
				str[1] - 'A' + 1, '-',                                      //M-
				str[2] <= '9' ? (str[2] - '0') : (str[2] - 'A' + 10),       //d
				' ',
				'Z' - str[3], ':',                                          //H:
				string.Concat(str[6], str[8]), ':',                         //mm:
				string.Concat(str[10], str[5])                              //ss
			);
			return DateTime.TryParse(makeup, out datetime);
		}
		public static bool IsValid(string? str) => TryParse(str, out _);

		//use 1 char to present Year, 0=2020, 1=2021, 2=2022 ...
		private static readonly int offsetYear = 2020;
		private static char Year => (char)('A' + (DateTime.Now.Year - offsetYear));

		//use 1 char to present Month, A=Jan, B=Feb, C=Mar ...
		private static char Month => (char)('A' + DateTime.Now.Month - 1);

		//use 1 char to present Day, 1=1, 2=2, ... A=10, B=11 ...
		private static char Day => (char)(DateTime.Now.Day + (DateTime.Now.Day < 10 ? '0' : 'A' - 10));

		//use 1 char to present Hour, Z=0, Y=1, X=2 ...
		private static char Hour => (char)('Z' - DateTime.Now.Hour);

		//use 7 chars to present Minutes and Seconds, the ordering is obfuscated
		private static string Time => string.Join("", TimePart());

		//append a validation digit number, which takes one char, from 0-9
		private static int Validation(string str) => str.Aggregate(0, (result, i) => result + i) * 9 % 10;


		private static int _antiDup = 0;
		private static IEnumerable<char> TimePart() {
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