using MediaPortal.Data.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Interface
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();        

        T Get(int id);      
    }
}
