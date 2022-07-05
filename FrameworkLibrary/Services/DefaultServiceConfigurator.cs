using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FrameworkLibrary.Services
{
    public class DefaultServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureContext(IServiceCollection services)
        {
            //TODO: configure any context here (i.e. loading user settings files, etc.)
        }

        public void ConfigureOptions(IServiceCollection services, IConfiguration configurationRoot)
        {
            services.Configure<ApplicationOptions>(configurationRoot.GetSection(nameof(ApplicationOptions)));
            services.Configure<UserOptions>(configurationRoot.GetSection(nameof(UserOptions)));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEnvironment, DefaultEnvironment>();
            services.AddSingleton<IFileSystemUtility, DefaultFileSystemUtility>();
        }
    }
}
