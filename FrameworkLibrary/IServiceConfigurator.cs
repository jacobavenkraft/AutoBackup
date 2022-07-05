using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FrameworkLibrary
{
    /// <summary>
    /// Implement this and use it in the extension call to App.Initialize in order to register all dependencies with
    /// the service collection for dependency injection support
    /// </summary>
    public interface IServiceConfigurator
    {
        void ConfigureServices(IServiceCollection services);
        void ConfigureOptions(IServiceCollection services, IConfiguration configurationRoot);
        void ConfigureContext(IServiceCollection services);
    }
}
