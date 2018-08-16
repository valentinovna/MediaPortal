using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThumbnailCreatorWebJob.Listeners;

using MediaPortal.BL;
using MediaPortal.BL.Infrastructure;
using MediaPortal.BL.Interface;
using MediaPortal.BL.Services;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Interface;
using MediaPortal.Data.Repositories;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using System.Configuration;
using System.Web;
using MediaPortal.Data.DataAccess;

namespace ThumbnailCreatorWebJob
{
    class Program
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        static void Main(string[] args)
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);

            IFileSystemService fileSystemService = bootstrapper.Kernel.Get<IFileSystemService>();
            var listener = new ThumbnailListener(fileSystemService);
            listener.Listen();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            // устанавливаем строку подключения
            //var modules = new INinjectModule[] { new ServiceModule("DefaultConnection") };
            //var kernel = new StandardKernel(modules);

            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //System.Web.Mvc.DependencyResolver.SetResolver(new MediaPortal.Util.NinjectDependencyResolver(kernel));

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string storageConnectionString = ConfigurationManager.ConnectionStrings["azureConnection"].ConnectionString;

            kernel.Bind<IFileSystemService>().To<FileSystemService>();

            kernel.Bind<IFileSystemRepository<FileSystem>>().To<FileSystemRepository>()
                .WithConstructorArgument("connectionString", connectionString);

            kernel.Bind<ITagRepository<Tag>>().To<TagRepository>()
                .WithConstructorArgument("connectionString", connectionString);

            kernel.Bind<IStorageRepository>().To<StorageDataAccess>()
                .WithConstructorArgument("storageConnectionString", storageConnectionString);
        }
    }
}
