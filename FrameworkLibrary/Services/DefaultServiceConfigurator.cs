using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Interop;
using FrameworkLibrary.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace FrameworkLibrary.Services
{
    public class DefaultServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureContext(IServiceCollection services)
        {
            //TODO: configure any context here (i.e. loading user settings files, etc.)
            //NOTE: there are no settings to load within the framework itself, it is up
            //to any app that includes the framework to load their app/user specific
            //settings within their own implementation of IServiceConfigurator
        }

        public void ConfigureOptions(IServiceCollection services, IConfiguration configurationRoot)
        {
            services.Configure<ApplicationOptions>(configurationRoot.GetSection(nameof(ApplicationOptions)));
            services.Configure<UserOptions>(configurationRoot.GetSection(nameof(UserOptions)));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IWindowFactory, WindowFactory>();
            services.AddTransient<IFolderPickerDialog, FolderPickerDialog>();
            services.AddSingleton<IEnvironment, DefaultEnvironment>();
            services.AddSingleton<IFileSystemUtility, DefaultFileSystemUtility>();
            services.AddSingleton<ICommonDialogService, CommonDialogService>();
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        }
    }
}
