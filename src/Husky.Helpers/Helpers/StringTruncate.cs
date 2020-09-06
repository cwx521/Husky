using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Husky
{
	public static class StringTruncate
	{
		public static string? SplitWords(this string? str) {
			if ( string.IsNullOrEmpty(str) ) {
				return str;
			}
			static bool IsCapital(char x) => x >= 'A' && x <= 'Z';
			var sb = new StringBuilder();
			for ( int i = 0; i < str.Length; i++ ) {
				var prevIsSpace = (i > 0) && str[i - 1] == ' ';
				var prevIsCapital = (i > 0) && IsCapital(str[i - 1]);
				var nextIsLowerCase = (i + 1 < str.Length) && !IsCapital(str[i + 1]);

				if (i != 0 && IsCapital(str[i]) && !prevIsSpace && (!prevIsCapital || nextIsLowerCase) ) {
					sb.Append(' ');
				}
				sb.Append(str[i]);
			}
			return sb.ToString();
		}

		public static string? StripWord(this string? str, string wordToRemove) {
			return string.IsNullOrEmpty(str) ? str : str.Replace(wordToRemove, "");
		}

		public static string? StripSpace(this string? str) {
			return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"\s+", "", RegexOptions.Multiline);
		}

		public static string? StripHtml(this string? str) {
			return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"<\/?[1-6a-zA-Z]+[^>]*>", "", RegexOptions.Multiline);
		}

		public static string? StripRegEx(this string? str, string pattern, RegexOptions options = RegexOptions.None) {
			return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, pattern, "", options);
		}

		public static string? Extract(this string? str, string pattern, int matchIndex = 1) {
			if ( string.IsNullOrEmpty(str) ) {
				return null;
			}
			var matches = new Regex(pattern).Matches(str);
			return matches.Count == 0 ? null : matches[Math.Max(matchIndex, 1) - 1].Value;
		}

		public static T Extract<T>(this string str, string pattern, int matchIndex = 1) where T : struct {
			return Extract(str, pattern, matchIndex).As<T>();
		}

		public static string? MidBy(this string? str, string afterKeyword, string endAtKeyword, bool useLastFoundAfterKeywordInsteadOfTheFirst = false) {
			if ( afterKeyword == null ) {
				throw new ArgumentNullException(nameof(afterKeyword));
			}
			if ( endAtKeyword == null ) {
				throw new ArgumentNullException(nameof(endAtKeyword));
			}
			if ( !string.IsNullOrEmpty(str) ) {
				var i = useLastFoundAfterKeywordInsteadOfTheFirst
					? str.LastIndexOf(afterKeyword, StringComparison.Ordinal)
					: str.IndexOf(afterKeyword, StringComparison.Ordinal);

				if ( i != -1 ) {
					i += afterKeyword.Length;
					var j = str.IndexOf(endAtKeyword, i, StringComparison.Ordinal);
					if ( j > i ) {
						return str[i..j];
					}
				}
			}
			return null;
		}

		public static string? LeftBy(this string? str, string beforeKeyword, bool useLastFoundKeywordInsteadOfTheFirst = false) {
			if ( beforeKeyword == null ) {
				throw new ArgumentNullException(nameof(beforeKeyword));
			}
			if ( !string.IsNullOrEmpty(str) ) {
				var i = useLastFoundKeywordInsteadOfTheFirst
					? str.LastIndexOf(beforeKeyword, StringComparison.Ordinal)
					: str.IndexOf(beforeKeyword, StringComparison.Ordinal);

				if ( i != -1 ) {
					return str.Substring(0, i);
				}
			}
			return null;
		}

		public static string? RightBy(this string? str, string afterKeyword, bool useLastFoundKeywordInsteadOfTheFirst = false) {
			if ( afterKeyword == null ) {
				throw new ArgumentNullException(nameof(afterKeyword));
			}
			if ( !string.IsNullOrEmpty(str) ) {
				var i = useLastFoundKeywordInsteadOfTheFirst
					? str.LastIndexOf(afterKeyword, StringComparison.Ordinal)
					: str.IndexOf(afterKeyword, StringComparison.Ordinal);

				if ( i != -1 ) {
					return str.Substring(i + afterKeyword.Length);
				}
			}
			return null;
		}

		public static string? Left(this string? str, int neededCount, bool endWithEllipsis = false) {
			return str == null || str.Length <= neededCount || neededCount < 3
				? str
				: endWithEllipsis
					? str.Substring(0, neededCount - 3) + "..."
					: str.Substring(0, neededCount);
		}

		public static string? LeftMonospaced(this string? str, int neededWidthInHalfangle) {
			if ( str == null || str.Length <= neededWidthInHalfangle / 2 || neededWidthInHalfangle < 3 ) {
				return str;
			}
			int i = 0, j = 0, counter = 0;
			while ( counter < neededWidthInHalfangle && i < str.Length - 1 ) {
				counter += str[i++] > 255 ? 2 : 1;
			}
			if ( counter <= neededWidthInHalfangle - 1 ) {
				return str;
			}
			while ( counter > neededWidthInHalfangle - 2 ) {
				counter -= str[i - j++] > 255 ? 2 : 1;
			}
			return str.Substring(0, i - j) + "...";
		}

		public static T[] Split<T>(this string? wellFormed, params char[] separators) where T : struct {
			return string.IsNullOrEmpty(wellFormed) ? new T[0] : wellFormed.Split(separators).Select(x => x.As<T>()).ToArray();
		}
	}
}