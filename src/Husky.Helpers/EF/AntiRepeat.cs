using System.ComponentModel.DataAnnotations;

namespace Husky
{
	public sealed class AntiRepeat
	{
		[Key]
		public string UniqueString { get; set; } = null!;
	}
}
