using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaPortal.Models
{
    public class UserFilesViewModels
    {
        public IList<MediaPortal.Models.FileSystemModels> Files { get; set;}
        public IList<int?> FolderIDs { get; set; }
        public IList<string> FolderNames { get; set; }
    }
}