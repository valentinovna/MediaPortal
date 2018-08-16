using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Models
{
    public class TagDTO
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public virtual IEnumerable<FileSystemDTO> FileSystems { get; set; }
    }
}
