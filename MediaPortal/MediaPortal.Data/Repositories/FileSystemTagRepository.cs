using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Data.DataAccess;
using MediaPortal.Data.Interface;
using MediaPortal.Data.EntitiesModel;

namespace MediaPortal.Data.Repositories
{
    public class FileSystemTagRepository : IRepository<FileSystemTag>
    {
        private string _connectionString;

        public FileSystemTagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public FileSystemTag Get(int userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return null;
            }
        }

        public IEnumerable<FileSystemTag> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return null;
            }
        }
        
    }
}
