using AutoBackup.Model;
using AutoBackup.ViewModel;
using AutoBackup.ViewModel.Windows;
using FrameworkLibrary;
using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AutoBackup
{
    public class AppServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureOptions(IServiceCollection services, IConfiguration configurationRoot)
        {
            //TODO: configure application options if needed
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IPersistenceService<ApplicationSettingsModel>, DefaultPersistenceService<ApplicationSettingsModel>>();
            services.AddTransient<ApplicationSettingsViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        public void ConfigureContext(IServiceCollection services)
        {
            services.AddSingleton<ApplicationSettingsModel>(x => x.GetService<IPersistenceService<ApplicationSettingsModel>>()?.LoadSettings() ?? new ApplicationSettingsModel());
        }
    }
}
