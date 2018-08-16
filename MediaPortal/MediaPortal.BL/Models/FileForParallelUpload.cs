using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MediaPortal.BL.Models
{
    public class FileForParallelUpload
    {
        public string UserID { get; set; }
        public int? ParrentID { get; set; }
        public HttpPostedFileWrapper File { get; set; }
        public string ModifiedDate { get; set; }
    }
}
