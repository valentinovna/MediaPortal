using System;
using MediaPortal.BL.Interface;
using MediaPortal.BL.Services;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Interface;
using MediaPortal.Data.Repositories;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using System.Configuration;
using System.Web;
using ZIPArchivatorWebJob.Listener;
using MediaPortal.Data.DataAccess;

namespace ZIPArchivatorWebJob
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
            var listener = new ArchiveListener(fileSystemService);

            listener.Listen().Wait();
        }

        private static IKernel CreateKernel()
        {
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

        private static void RegisterServices(IKernel kernel)
        {
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
