using AutoBackup.Model;
using FrameworkLibrary.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoBackup.ViewModel.Windows
{
    public class MainViewModel : ObservableObject
    {
        private readonly ILogger<MainViewModel> _logger;
        private ApplicationSettingsViewModel _settingsViewModel;
        private IPersistenceService<ApplicationSettingsModel> _persistenceService;

        public MainViewModel(ILogger<MainViewModel> logger, ApplicationSettingsViewModel settingsViewModel, IPersistenceService<ApplicationSettingsModel> persistenceService)
        {
            _logger = logger;
            _settingsViewModel = settingsViewModel;
            _persistenceService = persistenceService;

            persistenceService.SaveSettings(_settingsViewModel.Model);
        }

        public ApplicationSettingsViewModel Settings
        {
            get => _settingsViewModel;
            set => SetProperty(ref _settingsViewModel, value);
        }
    }
}
