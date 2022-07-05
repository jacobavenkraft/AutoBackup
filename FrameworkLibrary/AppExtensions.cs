using FrameworkLibrary.Services;
using FrameworkLibrary.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Windows;
using FrameworkLibrary.Options;
using Microsoft.Extensions.Options;

namespace FrameworkLibrary
{
    public static class AppExtensions
    {
        private static object _hostLock = new object();
        private static IHost? _host;

        public static void Initialize<TServiceConfigurator>(this Application application) where TServiceConfigurator : IServiceConfigurator, new()
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            lock (_hostLock)
            {
                if (_host != null)
                {
                    return;
                }

                var defaultConfigurator = new DefaultServiceConfigurator();
                var configurator = new TServiceConfigurator();

                _host = new HostBuilder()
                    .ConfigureAppConfiguration((context, configuration) =>
                    {
                        configuration.Sources.Clear();
                        //adding default first will allow appsettings.json to override default settings if needed
                        configuration
                            .AddJsonFile("appsettings.default.json", optional: true, reloadOnChange: true)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton(services);
                        defaultConfigurator.ConfigureServices(services);
                        configurator.ConfigureServices(services);

                        defaultConfigurator.ConfigureOptions(services, context.Configuration);
                        configurator.ConfigureOptions(services, context.Configuration);

                        defaultConfigurator.ConfigureContext(services);
                        configurator.ConfigureContext(services);
                    })
                    .Build();
            }

            application.Startup += async (s, e) => await StartupApplication();
            application.Exit += async (s, e) => await StopApplication();
        }

        private static async Task StartupApplication()
        {
            if (_host == null)
            {
                return;
            }

            await _host.StartAsync();

            IWindow? mainWindow = _host.Services.GetService<IMainWindow>();
            ApplicationOptions? options = _host.Services.GetService<IOptions<ApplicationOptions>>()?.Value;

            if (mainWindow == null)
            {
                //fallback provision: if there wasn't a specific IMainWindow registered then we
                //will look for the first Window class registered with a class name of MainWindow

                var serviceCollection = _host.Services.GetService<IServiceCollection>();

                if (serviceCollection != null)
                {
                    foreach (var service in serviceCollection)
                    {
                        var serviceType = service.ServiceType;

                        var mainWindowClassName = options?.MainWindowClassName ?? "MainWindow";

                        if (serviceType.Name == mainWindowClassName && typeof(Window).IsAssignableFrom(serviceType))
                        {
                            var instance = _host.Services.GetService(serviceType) as Window;

                            if (instance != null)
                            {
                                mainWindow = new WindowWrapper(instance);
                            }
                        }
                    }
                }
            }

            if (mainWindow == null)
            {
                throw new InvalidOperationException(@"Cannot show main window.  
                        ServiceCollection must have a registered IFrameworkMainWindow or 'MainWindow' class.
                        Please modify IServiceConfigurator instance to register a main window type.");
            }

            mainWindow.Show();
        }

        private static async Task StopApplication()
        {
            if (_host == null)
            {
                return;
            }

            ApplicationOptions options = _host.Services.GetService<IOptions<ApplicationOptions>>()?.Value ?? new ApplicationOptions();

            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(options.HostStopTimeoutSeconds));
            }

            _host = null;
        }
    }
}
