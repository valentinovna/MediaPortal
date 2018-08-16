using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaPortal.Models
{
    public class FileSystemModels
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public int? Size { get; set; }

        public string Type { get; set; }

        public string BlobLink { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime UploadDate { get; set; }

        public virtual IEnumerable<TagModels> Tags { get; set; }
    }
}