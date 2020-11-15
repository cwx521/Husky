using System;

namespace Husky
{
	public struct SocialIdNumber : IEquatable<SocialIdNumber>
	{
		public SocialIdNumber(string value) {
			Value = value;
		}

		public string Value { get; }

		public int? Age => !IsValid ? null : (int)Math.Ceiling(DateTime.Now.Subtract(DateOfBirth!.Value).TotalDays / 365);
		public Sex? Sex => !IsValid ? null : (Value[16] - '0') % 2 == 0 ? Husky.Sex.Female : Husky.Sex.Male;
		public DateTime? DateOfBirth => !IsValid ? null : DateTime.ParseExact(Value[6..14], "yyyyMMdd", null);

		public bool IsValid {
			get {
				if ( Value == null || Value.Length != 18 ) return false;
				if ( Value.Substring(6, 4).AsInt() < 1900 ) return false;
				if ( Value.Substring(10, 2).AsInt() > 12 ) return false;
				if ( Value.Substring(12, 2).AsInt() > 31 ) return false;
				var times = new[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
				var match = new[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
				var n = 0;
				for ( var i = 0; i < 17; n += (Value[i] - '0') * times[i++] ) ;
				return match[n % 11] == Value[17];
			}
		}

		public static bool operator ==(SocialIdNumber one, SocialIdNumber other) => one.Value == other.Value;
		public static bool operator !=(SocialIdNumber one, SocialIdNumber other) => one.Value != other.Value;

		public bool Equals(SocialIdNumber other) => Value == other.Value;
		public override bool Equals(object? obj) => obj is SocialIdNumber socialIdNumber && Equals(socialIdNumber);
		public override int GetHashCode() => base.GetHashCode();

		public override string ToString() => Value.ToString();
	}
}
