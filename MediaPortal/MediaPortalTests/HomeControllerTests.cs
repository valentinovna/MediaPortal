using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit;
using NSubstitute;
using NUnit.Framework;
using MediaPortal.Controllers;
using MediaPortal.BL.Interface;
using System.Collections.Generic;
using MediaPortal.BL.Models;

namespace MediaPortalTests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private IFileSystemService _fileSystemService;

        [SetUp]
        public void SetUp()
        {
            _fileSystemService = Substitute.For<IFileSystemService>();
            _controller = new HomeController(_fileSystemService);
        }
        
        //public void UserFiles_Correct()
        //{
        //    int? folderId = null;
        //    string folderName = "name";
        //    string userId = "id";

        //    var usersFolders = new List<FileSystemDTO>
        //    {
        //        new FileSystemDTO
        //        {
        //            Id = 1,
        //            Name = "n",
        //            Type = "Folder"
        //        },
        //        new FileSystemDTO
        //        {
        //            Id = 2,
        //            Name = "n2",
        //            Type = "Folder"
        //        }
        //    };

        //    _fileSystemService.GetUserFileSystem(userId, folderId).Returns(usersFolders);

        //    var result = _controller.UserFiles(folderId, folderName);
        //}
    }
}
