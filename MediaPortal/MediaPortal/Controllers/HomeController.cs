using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MediaPortal.BL;
using MediaPortal.BL.Interface;
using Serilog;
using MediaPortal.BL.Models;
using AutoMapper;
using MediaPortal.Models;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Threading.Tasks;

namespace MediaPortal.Controllers
{
    public class HomeController : Controller
    {
        private IFileSystemService _fileSystemService;

        public HomeController(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("UserFiles", "Home");
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult UserFiles(int? folderID)
        {
            string viewType = string.Empty;
            if (Request.Cookies["viewType"] != null)
            {
                viewType = Request.Cookies["viewType"].Value.ToString();
            }
            else
            {
                Response.Cookies["viewType"].Value = "List";
            }

            string sortType = string.Empty;
            if (Request.Cookies["sortType"] != null)
            {
                sortType = Request.Cookies["sortType"].Value.ToString();
            }
            else
            {
                Response.Cookies["sortType"].Value = "SortByAdditionDate";
            }

            string userId = User.Identity.GetUserId();
            List<FileSystemModels> files = null;
            IEnumerable<FileSystemDTO> fileSystemDtos;

            try
            {
                var folder = _fileSystemService.Get(userId, folderID);
                if (folder == null || folder.Type.ToLower().Equals("folder"))
                {
                    fileSystemDtos = _fileSystemService.GetUserFileSystem(userId, folderID);
                }
                else
                {
                    return HttpNotFound();
                }

            }
            catch (Exception ex)
            {
                // some logic for user
                return View("Error");
            }

            //Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>());

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagModels { Id = o.Id, Name = o.Name }).ToList())));

            files = Mapper.Map<IEnumerable<FileSystemDTO>, List<FileSystemModels>>(fileSystemDtos);



            var folderIDs = new List<int?>();
            var folderNames = new List<string>();

            if (folderID != null)
            {
                try
                {
                    var folder = _fileSystemService.Get(userId, folderID);

                    if (folder != null)
                    {
                        var tupleIdName = _fileSystemService.GetFoldersIdNamePair(folderID, userId);
                        folderIDs = tupleIdName.Item1;
                        folderNames = tupleIdName.Item2;
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                catch (Exception ex)
                {
                    // some logic for user
                    return View("Error");
                }
            }

            switch (sortType)
            {
                case "sortByUploadDate":
                    files = files.OrderBy(elem => elem.UploadDate).ToList();
                    break;
                case "sortByUploadDateDescending":
                    files = files.OrderByDescending(elem => elem.UploadDate).ToList();
                    break;
                case "sortByCreationDate":
                    files = files.OrderBy(elem => elem.CreationDate).ToList();
                    break;
                case "sortByCreationDateDescending":
                    files = files.OrderByDescending(elem => elem.CreationDate).ToList();
                    break;
                case "sortBySize":
                    files = files.OrderBy(elem => elem.Size).ToList();
                    break;
                case "sortBySizeDescending":
                    files = files.OrderByDescending(elem => elem.Size).ToList();
                    break;
                default:
                    break;
            }

            var viewModel = new UserFilesViewModels() { Files = files, FolderIDs = folderIDs, FolderNames = folderNames };

            if (viewType.Equals("BlockView"))
            {
                return View("UserFilesBlock", viewModel);
            }

            return View(viewModel);
        }

        [Authorize]
        public ActionResult ViewFile(int? fileSystemId, bool left = false, bool right = false)
        {
            if (fileSystemId == null)
            {
                return View("Error");
            }

            var userId = User.Identity.GetUserId();
            FileSystemModels fileSystem;
            FileSystemDTO fileSystemDTO;


            try
            {
                if (left)
                {
                    fileSystemDTO = _fileSystemService.Get(userId, fileSystemId);
                    var filesSystemDTO = _fileSystemService.GetUserFileSystem(userId, fileSystemDTO.ParentId).Select(element => element)
                                                           .Where(element => !element.Type.Equals("Folder"))
                                                           .ToList();
                    var index = filesSystemDTO.TakeWhile(element => !element.Id.Equals(fileSystemDTO.Id)).Count();
                    if (index - 1 < 0)
                    {
                        fileSystemDTO = filesSystemDTO.Last();
                    }
                    else
                    {
                        fileSystemDTO = filesSystemDTO.ElementAt(index - 1);
                    }
                }
                else if (right)
                {
                    fileSystemDTO = _fileSystemService.Get(userId, fileSystemId);
                    var filesSystemDTO = _fileSystemService.GetUserFileSystem(userId, fileSystemDTO.ParentId).Select(element => element)
                                                           .Where(element => !element.Type.Equals("Folder"))
                                                           .ToList();
                    var index = filesSystemDTO.TakeWhile(element => !element.Id.Equals(fileSystemDTO.Id)).Count();
                    if (index + 1 >= filesSystemDTO.Count)
                    {
                        fileSystemDTO = filesSystemDTO.FirstOrDefault();
                    }
                    else
                    {
                        fileSystemDTO = filesSystemDTO.ElementAt(index + 1);
                    }
                }
                else
                {
                    fileSystemDTO = _fileSystemService.Get(userId, fileSystemId);
                }
            }
            catch (Exception ex)
            {
                // some logic for user
                return View("Error");
            }

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>()
               .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagModels { Id = o.Id, Name = o.Name }).ToList())));


            fileSystem = Mapper.Map<FileSystemModels>(fileSystemDTO);

            return View(fileSystem);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SearchFiles(string searchValue)
        {
            string viewType = string.Empty;
            string searchType = string.Empty;
            if (Request.Cookies["viewType"] != null)
            {
                viewType = Request.Cookies["viewType"].Value.ToString();
            }
            else
            {
                Response.Cookies["viewType"].Value = "List";
            }
            if (Request.Cookies["searchType"] != null)
            {
                searchType = Request.Cookies["searchType"].Value.ToString();
            }
            else
            {
                Response.Cookies["searchType"].Value = "SearchByName";
            }

            string userId = User.Identity.GetUserId();
            List<FileSystemModels> files = null;
            IEnumerable<FileSystemDTO> allFileSystemDtos;
            IEnumerable<FileSystemDTO> fileSystemDtos;

            try
            {
                allFileSystemDtos = _fileSystemService.GetAllUserFileSystem(userId);

                if (searchType.Equals("SearchByName"))
                {
                    fileSystemDtos = allFileSystemDtos.Where(file => file.Name.ToLower().Contains(searchValue.ToLower()));
                }
                else
                {
                    fileSystemDtos = allFileSystemDtos.Where(file => file.Tags.Any(tag => tag.Name.ToLower().Contains(searchValue.ToLower())));
                }

                if (!fileSystemDtos.Any())
                {
                    return View(viewName: "SearchZeroResult");
                }
            }
            catch (Exception)
            {
                // some logic for user
                return View("Error");
            }

            //Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>());

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagModels { Id = o.Id, Name = o.Name }).ToList())));

            files = Mapper.Map<IEnumerable<FileSystemDTO>, List<FileSystemModels>>(fileSystemDtos);

            var viewModel = new UserFilesViewModels() { Files = files, FolderIDs = new List<int?>(), FolderNames = new List<string>() };

            if (viewType.Equals("BlockView"))
            {
                return View("UserFilesBlock", viewModel);
            }

            return View("UserFiles", viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateFolder(FileSystemModels model, string returnUrl)
        {
            Response.Cookies["registered"].Expires = DateTime.Now.AddDays(-1);
            model.UserId = User.Identity.GetUserId();
            model.Type = "Folder";
            model.CreationDate = DateTime.Now;
            model.UploadDate = DateTime.Now;

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemModels, FileSystemDTO>());
            var fileSystem = Mapper.Map<FileSystemDTO>(model);

            try
            {
                _fileSystemService.InsertFileSystem(fileSystem);
            }
            catch (DataException)
            {
                // some logic for user
                return View("Error");
            }
            if (model.ParentId == null)
            {
                return RedirectToAction("UserFiles");
            }
            else
            {
                return RedirectToAction("UserFiles", new { folderID = model.ParentId, folderName = model.Name });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadFiles(FilesToUploadModels model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_FilesToUploadPartial", model);
            }

            Response.Cookies["registered"].Expires = DateTime.Now.AddDays(-1);
            Mapper.Initialize(cfg => cfg.CreateMap<FilesToUploadModels, FilesToUploadDTO>());
            var filesToUpload = Mapper.Map<FilesToUploadDTO>(model);
            try
            {
                _fileSystemService.UploadAndInsertFiles(filesToUpload);
            }
            catch (Exception ex)
            {
                return View("Error");
            }

            return Content("Files has been uploaded");
            //return RedirectToAction("UserFiles", new { folderID = model.ParrentID });
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> DeleteFileSystem(int[] fileSystemsId)
        {
            try
            {
                await _fileSystemService.DeleteFileSystem(fileSystemsId);
            }
            catch (DataException)
            {
                return Json("false");
            }

            return Json("true");
        }

        [Authorize]
        public async Task<FileResult> DownloadFileSystem(int fileSystemId, string fileSystemName)
        {
            byte[] fileBytes;

            try
            {
                var tupleFileBytes = await _fileSystemService.DownloadFileSystem(fileSystemId);
                fileBytes = tupleFileBytes.Item1;
                fileSystemName += tupleFileBytes.Item2;
            }
            catch (Exception ex)
            {
                // some logic for user
                return null;
            }

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileSystemName);
        }


        [Authorize]
        //[HttpPost]
        public JsonResult DownloadFileSystemZIP(List<int> fileSystemsId)
        {
            string userId = User.Identity.GetUserId();
            string ZIParchiveName = null;

            try
            {
                ZIParchiveName = _fileSystemService.DownloadFileSystemZIP(fileSystemsId, userId);
            }
            catch (DataException)
            {
                // some logic for user
                return null;
            }

            return Json(ZIParchiveName);
        }

        [Authorize]
        //[HttpPost]
        public JsonResult ZIPisReady(string fileSystemName)
        {
            bool blobIsExist = false;

            while (!blobIsExist)
            {
                try
                {
                    blobIsExist = _fileSystemService.IsExistArchive(fileSystemName);
                }
                catch (Exception ex)
                {
                    // some logic for user                    
                }
            }

            return Json(blobIsExist);
        }

        [Authorize]
        //[HttpPost]
        public async Task<ActionResult> DownloadProcess(string fileSystemName)
        {
            byte[] ZIPFileBytes;

            try
            {
                ZIPFileBytes = await _fileSystemService.DownloadProcessZIP(fileSystemName);

                await _fileSystemService.DeleteFileSystemByName(fileSystemName);
            }
            catch (Exception ex)
            {
                // some logic for user
                return View("Error");
            }

            if (ZIPFileBytes == null)
            {
                return View("Error");
            }
            else
            {
                return File(ZIPFileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileSystemName + ".zip");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult RenameFileSystem(int fileSystemId, string textName)
        {
            try
            {
                _fileSystemService.RenameFileSystem(fileSystemId, textName);
            }
            catch (DataException)
            {
                // some logic for user
            }

            return Json("true");
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> AddTag(int[] fileSystemId, string tagValue)
        {
            try
            {
                await _fileSystemService.AddTagAsync(fileSystemId, tagValue);
            }
            catch (DataException)
            {
                // some logic for user
            }

            return Json("true");
        }

        [Authorize]
        [HttpPost]
        public JsonResult MoveFileSystem(List<int> fileSystemsId, int? fileSystemParentId)
        {
            string userId = User.Identity.GetUserId();
            bool moved = false;

            try
            {
                moved = _fileSystemService.MoveFileSystem(fileSystemsId, fileSystemParentId, userId);
            }
            catch (DataException)
            {
                // some logic for user
            }

            return Json(moved);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View("Error");
        }

        public async Task<ActionResult> GetThumbnailImage(int fileSystemId)
        {
            string userId = User.Identity.GetUserId();
            var fileImageStream = await _fileSystemService.GetFileSystemThumbnailAsync(userId, fileSystemId);

            if (fileImageStream != null)
            {
                return File(fileImageStream, "image/png");
            }

            return null;
        }

        public async Task<ActionResult> GetImage(int fileSystemId)
        {
            string userId = User.Identity.GetUserId();
            var fileImageStream = await _fileSystemService.GetFileSystemStreamAsync(userId, fileSystemId);

            return File(fileImageStream, "image/png");
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetVideo(int fileSystemID)
        {
            string userId = User.Identity.GetUserId();
            string urlBlob = string.Empty;
            try
            {
                var fileSystemDTO = _fileSystemService.Get(userId, fileSystemID);
                urlBlob = _fileSystemService.SetFileReadPermission(fileSystemDTO.BlobLink, timeToExpire: 60);
            }
            catch (Exception ex)
            {
                // some logic for user
                return View("Error");
            }
            ViewData["VideoUrl"] = urlBlob;
            return PartialView("_VideoPartial");
        }

        [Authorize]
        [HttpPost]
        public JsonResult GiveAccessLink(int fileSystemId)
        {
            string userId = User.Identity.GetUserId();
            string urlBlob = string.Empty;

            try
            {
                var fileSystemDTO = _fileSystemService.Get(userId, fileSystemId);
                urlBlob = _fileSystemService.SetFileReadPermission(fileSystemDTO.BlobLink, timeToExpire: 60);
            }
            catch (Exception ex)
            {
                // some logic for user               
            }

            return Json(urlBlob);
        }
    }
}