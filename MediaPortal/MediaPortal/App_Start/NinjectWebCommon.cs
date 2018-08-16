using MediaPortal.BL;
using MediaPortal.BL.Infrastructure;
using MediaPortal.BL.Interface;
using MediaPortal.BL.Services;
using MediaPortal.Data.DataAccess;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Interface;
using MediaPortal.Data.Repositories;
using Microsoft.Azure;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using System;
using System.Configuration;
using System.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MediaPortal.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(MediaPortal.App_Start.NinjectWebCommon), "Stop")]

namespace MediaPortal.App_Start
{
    public class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
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
            string storageConnectionString = CloudConfigurationManager.GetSetting("teamresponse200_AzureStorageConnectionString");

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