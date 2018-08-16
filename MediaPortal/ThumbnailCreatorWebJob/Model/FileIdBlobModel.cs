using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ThumbnailCreatorWebJob.Model
{
    [DataContract]
    public class FileIdBlobModel
    {
        [DataMember]
        public int FileId { get; set; }

        [DataMember]
        public string BlobLink { get; set; }
    }
}
