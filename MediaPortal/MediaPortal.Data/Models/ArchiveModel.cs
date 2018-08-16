using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MediaPortal.Data.Models
{
    [DataContract]
    public class ArchiveModel
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public List<int> FileSystemsId { get; set; }
    }
}
