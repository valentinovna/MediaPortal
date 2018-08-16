using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Models
{
    public class SharedAccessDTO
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        public string FileSystemId { get; set; }
        
        public string Link { get; set; }

        public DateTime ExpirationDate { get; set; }        
    }
}
