using MediaPortal.Data.DataAccess;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Repositories
{
    public class AspNetUserRepositories : IRepository<AspNetUser>
    {
        private string _connectionString;

        public AspNetUserRepositories(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AspNetUser Get(int userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.AspNetUsers.Find(userId);
            }
        }

        public IEnumerable<AspNetUser> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.AspNetUsers.ToList();
            }
        }
        
    }
}
