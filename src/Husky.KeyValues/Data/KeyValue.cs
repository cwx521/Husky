using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Husky.KeyValues.Data
{
	public class KeyValue
	{
		[Key, StringLength(50), Unicode(false), Required]
		public string Key { get; init; } = null!;

		[StringLength(2000)]
		public string? Value { get; set; }
	}
}
