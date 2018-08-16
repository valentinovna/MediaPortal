using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MediaPortal.Data.Interface
{
    public interface IStorageRepository
    {
        Task UploadFileInBlocksAsync(byte[] file, string guid);

        Task<string> UploadFileInBlocksAsync(HttpPostedFileWrapper file);

        Task<Stream> GetFileStream(string blobLink);        

        Task<byte[]> DownloadFile(string blobLink);

        bool IsExistArchive(string blobLink);

        Task DeleteFileSystem(string blobLink);

        string SetFileReadPermission(string blobLink, int timeToExpire);    

        void PutMessageRequestForThumbnail(int id, string blobUri);

        void PutMessageRequestForZIPArchivator(string ZIPId, List<int> fileSystemsId, string userId);
    }
}
