using MediaPortal.CustomValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaPortal.Models
{
    public class FilesToUploadModels
    {
        public string UserID { get; set; }
        public int? ParrentID { get; set; }

        [FileExtensionValidation(ErrorMessage = "Alowed content with extension: .png, .jpg, .mp4")]
        [FileMaximumSizeValidation(ErrorMessage = "For video Max Size 50MB. For images Max Size 2MB")]
        public IList<HttpPostedFileWrapper> Files { get; set; }
        public IList<string> ModifiedDates { get; set; }
    }
}