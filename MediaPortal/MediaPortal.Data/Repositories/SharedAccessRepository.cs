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
    public class SharedAccessRepository : IRepository<SharedAccess>
    {
        private string _connectionString;

        public SharedAccessRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SharedAccess Get(int userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.SharedAccesses.Find(userId);
            }
        }

        public IEnumerable<SharedAccess> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.SharedAccesses.ToList();
            }
        }        
    }
}
