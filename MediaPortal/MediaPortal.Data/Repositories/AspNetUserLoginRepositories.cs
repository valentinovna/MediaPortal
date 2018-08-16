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
    public class AspNetUserLoginRepositories : IRepository<AspNetUserLogin>
    {
        private string _connectionString;

        public AspNetUserLoginRepositories(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AspNetUserLogin Get(int userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.AspNetUserLogins.Find(userId);
            }
        }

        public IEnumerable<AspNetUserLogin> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.AspNetUserLogins.ToList();
            }
        }        
    }
}
