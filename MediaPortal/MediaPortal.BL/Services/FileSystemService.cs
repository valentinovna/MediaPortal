using MediaPortal.Data.Interface;
using MediaPortal.Data.Repositories;
using MediaPortal.Data;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.BL.Interface;
using MediaPortal.BL.Models;
using MediaPortal.Data.EntitiesModel;
using AutoMapper;
using MediaPortal.BL.Infrastructure;
using System.Diagnostics;
using System.Data;
using MediaPortal.Data.DataAccess;
using System.Web;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Globalization;

namespace MediaPortal.BL.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystemRepository<FileSystem> _fileSystemRepository;

        private readonly ITagRepository<Tag> _tagRepository;

        private readonly IStorageRepository _storageRepository;

        public FileSystemService(IFileSystemRepository<FileSystem> fileSyatemRepository, ITagRepository<Tag> tagRepository, IStorageRepository storageRepository)
        {
            _fileSystemRepository = fileSyatemRepository;
            _tagRepository = tagRepository;

            _storageRepository = storageRepository;
        }        

        public IEnumerable<FileSystemDTO> GetAllUserFileSystem(string userId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));

            var fileSystems = _fileSystemRepository.GetAll(userId);

            return Mapper.Map<IEnumerable<FileSystem>, List<FileSystemDTO>>(fileSystems);
        }

        public IEnumerable<FileSystemDTO> GetUserFileSystem(string userId, int? fileSystemParentId = null)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));

            IEnumerable<FileSystem> fileSystem = null;
            try
            {
                fileSystem = _fileSystemRepository.GetAll(userId, fileSystemParentId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
            return Mapper.Map<IEnumerable<FileSystem>, IEnumerable<FileSystemDTO>>(fileSystem);
        }

        public void InsertFileSystem(FileSystemDTO model)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystem>());

            var fileSystem = Mapper.Map<FileSystem>(model);

            try
            {
                var folderExist = _fileSystemRepository.Get(fileSystem.UserId, fileSystem.ParentId, fileSystem.Name);

                if(folderExist)
                {
                    _fileSystemRepository.InsertObject(fileSystem);
                }                
            }
            catch (DataException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public Tuple<List<int?>, List<string>> GetFoldersIdNamePair(int? folderID, string userId)
        {
            var folderIDs = new List<int?>();
            var folderNames = new List<string>();

            IEnumerable<FileSystemDTO> allFiles = new List<FileSystemDTO>();

            folderIDs.Add(folderID);

            try
            {
                allFiles = GetAllUserFileSystem(userId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }

            while (folderID != null)
            {
                var parent = allFiles.Where(file => file.Id == folderID).FirstOrDefault();

                if (parent != null)
                {
                    var parentID = parent.ParentId;

                    if (parentID != null)
                    {
                        folderIDs.Add(parentID);
                    }
                    folderID = parentID;
                }
            }

            foreach (int? id in folderIDs)
            {
                var name = allFiles.Where(file => file.Id == id).FirstOrDefault().Name;
                folderNames.Add(name);
            }

            folderIDs.Reverse();
            folderNames.Reverse();

            return new Tuple<List<int?>, List<string>>(folderIDs, folderNames);
        }

        public async Task DeleteFileSystem(int[] fileSystemsId)
        {
            try
            {
                foreach (var fileSystemId in fileSystemsId)
                {
                    var FileSystemsForDelete = _fileSystemRepository.SearchFileSystemForDelete(fileSystemId);

                    if (FileSystemsForDelete != null)
                    {
                        FileSystemsForDelete.Reverse();

                        foreach (var fileSystem in FileSystemsForDelete)
                        {
                            _fileSystemRepository.DeleteFileSystem(fileSystem.Id);

                            if (fileSystem.BlobLink != null)
                            {
                                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobLink;
                                await _storageRepository.DeleteFileSystem(blobLink);

                                if (fileSystem.BlobThumbnail != null)
                                {
                                    var blobThumbnailLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobThumbnail;
                                    await _storageRepository.DeleteFileSystem(blobThumbnailLink);
                                }
                            }
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }
        }

        public async Task<byte[]> DownloadProcessZIP(string fileSystemName)
        {
            byte[] fileBytes;

            try
            {
                string containerName = ConfigurationManager.AppSettings.Get("containerNameAzureStorageBlob");
                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + containerName + fileSystemName + ".zip";

                fileBytes = await _storageRepository.DownloadFile(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }

            return fileBytes;
        }

        public bool IsExistArchive(string fileSystemName)
        {
            bool existArchive = false;

            try
            {
                string containerName = ConfigurationManager.AppSettings.Get("containerNameAzureStorageBlob");
                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + containerName + fileSystemName + ".zip";

                existArchive = _storageRepository.IsExistArchive(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }

            return existArchive;
        }

        public async Task DeleteFileSystemByName(string fileSystemName)
        {
            try
            {
                string containerName = ConfigurationManager.AppSettings.Get("containerNameAzureStorageBlob");
                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + containerName + fileSystemName + ".zip";

                await _storageRepository.DeleteFileSystem(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }            
        }

        public async Task<Tuple<byte[], string>> DownloadFileSystem(int fileSystemId)
        {
            byte[] fileBytes;
            string fileType;

            try
            {
                var fileSystem = _fileSystemRepository.Get(fileSystemId);

                fileType = fileSystem.Type;

                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobLink;

                fileBytes = await _storageRepository.DownloadFile(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }

            return new Tuple<byte[], string>(fileBytes, fileType);
        }

        public string DownloadFileSystemZIP(List<int> fileSystemsId, string userId)
        {
            string ZIParchiveId = Guid.NewGuid().ToString();
            
            try
            {
                _storageRepository.PutMessageRequestForZIPArchivator(ZIParchiveId, fileSystemsId, userId);                
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }            

            return ZIParchiveId;
        }

        public FileSystemDTO Get(string userId,int? fileSystemId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));
            
            FileSystem fileSystem = null;
            try
            {
                fileSystem = _fileSystemRepository.Get(userId,fileSystemId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);                
            }

            return Mapper.Map<FileSystem, FileSystemDTO>(fileSystem);
        }

        public FileSystemDTO Get(int fileSystemId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));

            FileSystem fileSystem = null;
            try
            {
                fileSystem = _fileSystemRepository.Get(fileSystemId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return Mapper.Map<FileSystem, FileSystemDTO>(fileSystem);
        }

        private async Task<byte[]> DownloadFile(string blobLink)
        {
            byte[] fileBytes = null;

            try
            {
                fileBytes = await _storageRepository.DownloadFile(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return fileBytes;
        }

        public IEnumerable<FileSystemDTO> GetAll(string userId, int fileSystemId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));

            IEnumerable<FileSystem> fileSystem = null;

            try
            {
                fileSystem = _fileSystemRepository.GetAll(userId, fileSystemId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return Mapper.Map<IEnumerable<FileSystem>, IEnumerable<FileSystemDTO>>(fileSystem);
        }               

        public void RenameFileSystem(int fileSystemId, string name)
        {
            try
            {
                _fileSystemRepository.RenameFileSystem(fileSystemId, name);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task<Stream> GetFileSystemThumbnailAsync(string userID,int fileSystemId)
        {
            var fileSystem = _fileSystemRepository.Get(userID,fileSystemId);

            if(fileSystem.BlobThumbnail != null)
            {
                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobThumbnail;

                Stream fileStream = null;

                try
                {
                    fileStream = await _storageRepository.GetFileStream(blobLink);
                    return fileStream;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.InnerException.Message);
                }

                return null;
            }     
            else
            {
                var tempPathToFile = HttpContext.Current.Server.MapPath("~/Content/icons/loadingImage.jpg");

                Stream standartImageStream = null;

                try
                {
                    standartImageStream = File.Open(tempPathToFile, FileMode.Open);  
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.InnerException.Message);
                }

                return standartImageStream;
            }            
        }

        public async Task<Stream> GetFileSystemStreamAsync(string userID, int fileSystemId)
        {
            var fileSystem = _fileSystemRepository.Get(userID, fileSystemId);
            var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobLink;
            Stream fileStream;
            try
            {
                fileStream = await _storageRepository.GetFileStream(blobLink);
                return fileStream;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
            }

            return null;

        }        

        public void UploadAndInsertFiles(FilesToUploadDTO filesToUpload)
        {
            var objectForParallelUpload = new ConcurrentBag<FileForParallelUpload>();
            for (var i = 0; i < filesToUpload.Files.Count; i++)
            {
                var file = new FileForParallelUpload
                {
                    UserID = filesToUpload.UserID,
                    ParrentID = filesToUpload.ParrentID,
                    File = filesToUpload.Files[i],
                    ModifiedDate = filesToUpload.ModifiedDates[i]
                };
                objectForParallelUpload.Add(file);
            }
            Parallel.ForEach(objectForParallelUpload, obj =>
            {
                if (obj != null)
                {
                    if( _fileSystemRepository.Get(filesToUpload.UserID, filesToUpload.ParrentID, Path.GetFileNameWithoutExtension(obj.File.FileName)))
                    {
                        var uri = _storageRepository.UploadFileInBlocksAsync(obj.File).Result;

                        var cuttedUri = GetFileCuttedUri(uri);

                        DateTime modifiedDate;
                        double amountOfMiliseconds;
                        if (!double.TryParse(obj.ModifiedDate, out amountOfMiliseconds))
                        {
                            modifiedDate = DateTime.Now;
                        }
                        else
                        {
                            modifiedDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(amountOfMiliseconds);
                        }

                        var fileSystem = new FileSystem()
                        {
                            UserId = obj.UserID,
                            ParentId = obj.ParrentID,
                            Name = Path.GetFileNameWithoutExtension(obj.File.FileName),
                            Type = Path.GetExtension(obj.File.FileName).ToLower(),
                            Size = obj.File.ContentLength,
                            BlobLink = cuttedUri,
                            UploadDate = modifiedDate,
                            CreationDate = DateTime.Now
                        };                        

                        var insertedId = _fileSystemRepository.InsertObject(fileSystem);

                        _storageRepository.PutMessageRequestForThumbnail(insertedId, uri);
                    }                    
                }
            });
        }

        private string GetFileCuttedUri(string blobLink)
        {
            var cuttedLink = blobLink.Replace(ConfigurationManager.AppSettings.Get("azureStorageBlobLink"), "");
            return cuttedLink;
        }

        public async Task AddTagAsync(int[] fileSystemId, string tagValue)
        {
            for (int i = 0; i < fileSystemId.Length; i++)
            {
                var fileSystem = _fileSystemRepository.Get(fileSystemId[i]);

                if (fileSystem != null)
                {
                    string pattern = " ";
                    string replacement = "_";
                    string tagName = Regex.Replace(tagValue, pattern, replacement);

                    var tag = _tagRepository.Get(tagName);

                    int tagId;

                    if (tag == null)
                    {
                        var tagObj = new Tag()
                        {
                            Name = tagName,
                        };

                        tagId = _tagRepository.InsertObject(tagObj);

                        var currentTag = _tagRepository.Get(tagId);

                        await _fileSystemRepository.AddTagAsync(fileSystemId[i], currentTag.Id);
                    }
                    else
                    {
                        await _fileSystemRepository.AddTagAsync(fileSystemId[i], tag.Id);
                    }
                }
            }
        }

        public bool MoveFileSystem(List<int> fileSystemsId, int? fileSystemParentId, string userId)
        {
            bool unique = true;

            try
            {
                foreach (var fileSystemId in fileSystemsId)
                {
                    var current = _fileSystemRepository.Get(fileSystemId);
                            
                    if(current == null)
                    {
                        unique = false;
                        break;
                    }

                    unique = _fileSystemRepository.UniqueName(current.Name, fileSystemParentId, userId);
                    if (!unique)
                    {
                        break;
                    }   
                }

                if(unique)
                {
                    Parallel.ForEach(fileSystemsId, fileSystemId =>
                    {
                        _fileSystemRepository.ChangeFileSystemParentId(fileSystemId, fileSystemParentId, userId);
                    });
                }                
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return unique;
        }

        public void UpdateThumbnail(int id, string thumbnailUri)
        {
            _fileSystemRepository.FileSystemAddThumbnailLink(id, thumbnailUri);
        }

        public string SetFileReadPermission(string blobLink, int timeToExpire)
        {
            return _storageRepository.SetFileReadPermission(ConfigurationManager.AppSettings.Get("azureStorageBlobLink")+blobLink, timeToExpire);
        }
    }
}
