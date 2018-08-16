using MediaPortal.BL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Interface
{
    public interface IFileSystemService
    {
        IEnumerable<FileSystemDTO> GetAllUserFileSystem(string userId);

        IEnumerable<FileSystemDTO> GetUserFileSystem(string userId, int? fileSystemParentId = null);

        Tuple<List<int?>, List<string>> GetFoldersIdNamePair(int? folderID, string userId);

        void InsertFileSystem(FileSystemDTO model);

        Task DeleteFileSystem(int[] fileSystemsId);

        Task DeleteFileSystemByName(string fileSystemName);

        Task<byte[]> DownloadProcessZIP(string fileSystemName);

        bool IsExistArchive(string fileSystemName);

        Task<Tuple<byte[], string>> DownloadFileSystem(int fileSystemId);

        string DownloadFileSystemZIP(List<int> fileSystemsId, string userId);

        FileSystemDTO Get(int fileSystemId);

        FileSystemDTO Get(string userID,int? fileSystemId);        

        IEnumerable<FileSystemDTO> GetAll(string userId, int fileSystemId);                

        void RenameFileSystem(int fileSystemId, string name);

        Task AddTagAsync(int[] fileSystemId, string tagValue);

        bool MoveFileSystem(List<int> fileSystemsId, int? fileSystemParentId, string userId);

        void UploadAndInsertFiles(FilesToUploadDTO filesToUpload);

        Task<Stream> GetFileSystemThumbnailAsync(string userId,int fileSystemId);

        Task<Stream> GetFileSystemStreamAsync(string userId, int fileSystemId);

        void UpdateThumbnail(int id, string thumbnailUri);

        string SetFileReadPermission(string blobLink, int timeToExpire);
    }
}
