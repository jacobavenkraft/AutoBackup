using AutoBackup.Model;
using FrameworkLibrary.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace AutoBackup.ViewModel.Windows
{
    public class MainViewModel : ObservableObject
    {
        private readonly ILogger<MainViewModel> _logger;
        private ApplicationSettingsViewModel _settingsViewModel;
        private IPersistenceService<ApplicationSettingsModel> _persistenceService;

        private IRelayCommand? _saveChangesCommand;

        public MainViewModel(ILogger<MainViewModel> logger, ApplicationSettingsViewModel settingsViewModel, IPersistenceService<ApplicationSettingsModel> persistenceService)
        {
            _logger = logger;
            _settingsViewModel = settingsViewModel;
            _persistenceService = persistenceService;

            persistenceService.SaveSettings(_settingsViewModel.Model);
        }

        public IRelayCommand SaveChangesCommand => _saveChangesCommand ??= new RelayCommand(DoSaveChanges);

        public ApplicationSettingsViewModel Settings
        {
            get => _settingsViewModel;
            set => SetProperty(ref _settingsViewModel, value);
        }

        private void DoSaveChanges() => _persistenceService.SaveSettings(_settingsViewModel.Model);
    }
}
