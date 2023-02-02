using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Husky.FileStore.Data
{
    public class StoredFile
    {
        [Key]
        public int Id { get; set; }

        public Guid? AnonymousId { get; set; }

        public int? UserId { get; set; }

        [StringLength(50)]
        public string? UserName { get; set; }

        [StringLength(200), Required, NeverUpdate, Unique]
        public string FileName { get; set; } = null!;

        public long FileContentLength { get; set; }

        public StoredFileType FileType { get; set; }

        public StoredFileAccessControl AccessControl { get; set; }

        public OssProvider StoredAt { get; set; }

        public bool IsDeleted { get; set; }

        [DefaultValueSql("getdate()"), NeverUpdate]
        public DateTime CreatedTime { get; init; } = DateTime.Now;
    }
}
