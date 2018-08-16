using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Models
{
    public class FileDeleteModel
    {
        public int Id { get; set; }

        public string BlobLink { get; set; }
        
        public string BlobThumbnail { get; set; }
    }
}
