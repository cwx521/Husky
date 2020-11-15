using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husky.FileStore.Data
{
	public class StoredFileTag
	{
		[Key]
		public int Id { get; set; }

		public int StoredFileId { get; set; }

		[StringLength(50), Required]
		public string Key { get; set; } = null!;

		[StringLength(200), Required]
		public string Value { get; set; } = null!;
	}
}
