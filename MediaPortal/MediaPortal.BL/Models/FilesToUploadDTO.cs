using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MediaPortal.BL.Models
{
    public class FilesToUploadDTO
    {
        public string UserID { get; set; }
        public int? ParrentID { get; set; }
        public IList<HttpPostedFileWrapper> Files { get; set; }
        public IList<string> ModifiedDates { get; set; }
    }
}
