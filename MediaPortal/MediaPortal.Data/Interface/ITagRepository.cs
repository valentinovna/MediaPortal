using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Interface
{
    public interface ITagRepository<T> : IRepository<T> where T : class
    {
        T Get(string name);

        int InsertObject(T t);
    }
}
