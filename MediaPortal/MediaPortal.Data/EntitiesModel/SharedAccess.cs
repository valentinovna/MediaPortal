namespace MediaPortal.Data.EntitiesModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SharedAccess")]
    public partial class SharedAccess
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public int FileSystemId { get; set; }

        [Required]
        [StringLength(256)]
        public string Link { get; set; }

        public DateTime ExpirationDate { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual FileSystem FileSystem { get; set; }
    }
}
