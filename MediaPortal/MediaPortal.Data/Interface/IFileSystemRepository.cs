using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Interface
{
    public interface IFileSystemRepository<T> : IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string userId);

        FileSystem Get(string userId, int? id);

        bool Get(string userId, int? parentId, string name);

        IEnumerable<T> GetAll(string userId, int? fileSystemParentId);

        int InsertObject(T t);

        List<FileDeleteModel> SearchFileSystemForDelete(int fileSystemId);

        void DeleteFileSystem(int fileSystemId);

        void RenameFileSystem(int fileSystemId, string name);

        void ChangeFileSystemParentId(int fileSystemId, int? fileSystemParentId, string userId);

        bool UniqueName(string name, int? fileSystemParentId, string userId);

        void FileSystemAddThumbnailLink(int fileSystemId, string link);

        Task AddTagAsync(int fileSystem, int tag);
    }
}
