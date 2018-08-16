namespace MediaPortal.Data.EntitiesModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FileSystem")]
    public partial class FileSystem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FileSystem()
        {
            FileSystem1 = new HashSet<FileSystem>();
            SharedAccesses = new HashSet<SharedAccess>();
            Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int? ParentId { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public int? Size { get; set; }

        [Required]
        [StringLength(256)]
        public string Type { get; set; }

        [StringLength(256)]
        public string BlobLink { get; set; }

        [StringLength(256)]
        public string BlobThumbnail { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileSystem> FileSystem1 { get; set; }

        public virtual FileSystem FileSystem2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SharedAccess> SharedAccesses { get; set; }

        //private ICollection<Tag> _tags;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tag> Tags { get; set; }
        //public virtual ICollection<Tag> Tags
        //{
        //    get { return _tags ?? (_tags = new Collection<Tag>()); }
        //    set { _tags = value; }
        //}
    }
}
