using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Models
{
    public class FileSystemTagDTO
    {
        public string FileSystemId { get; set; }
        
        public int TagId { get; set; }
    }
}
