using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaPortal.Models
{
    public class TagModels
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<FileSystemModels> FileSystems { get; set; }
    }
}