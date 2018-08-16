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
    public class TagRepository : ITagRepository<Tag>
    {
        private string _connectionString;

        public TagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Tag Get(int userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.Tags.Find(userId);
            }
        }

        public Tag Get(string name)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.Tags.Where(x => x.Name == name).FirstOrDefault();
            }
        }

        public IEnumerable<Tag> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.Tags.ToList();
            }
        }

        public int InsertObject(Tag tag)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                dbContext.Tags.Add(tag);

                dbContext.SaveChanges();

                return tag.Id;
            }
        }

    }
}
