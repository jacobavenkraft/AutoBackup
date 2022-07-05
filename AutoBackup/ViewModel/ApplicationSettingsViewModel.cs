using AutoBackup.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AutoBackup.ViewModel
{
    public class ApplicationSettingsViewModel : ObservableObject
    {
        public ApplicationSettingsModel Model { get; }

        private PathMappingViewModel? _selectedPathMapping;
        private IRelayCommand? _addPathMappingCommand;
        private IRelayCommand? _deleteSelectedPathMappingCommand;

        public ApplicationSettingsViewModel(ApplicationSettingsModel model)
        {
            Model = model;
            ClonePathMappings();
        }

        public ObservableCollection<PathMappingViewModel> PathMappings { get; } = new ObservableCollection<PathMappingViewModel>();

        public IRelayCommand AddPathMappingCommand => _addPathMappingCommand ??= new RelayCommand(DoAddPathMapping);
        public IRelayCommand DeleteSelectedPathMappingCommand => _deleteSelectedPathMappingCommand ??= new RelayCommand(DoDeleteSelectedPathMapping, CanDeleteSelectedPathMapping);

        public PathMappingViewModel? SelectedPathMapping
        {
            get => _selectedPathMapping;
            set
            {
                if (SetProperty(ref _selectedPathMapping, value))
                {
                    DeleteSelectedPathMappingCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private void ClonePathMappings()
        {
            foreach (var mapping in Model.PathMappings)
            {
                PathMappings.Add(new PathMappingViewModel(mapping));
            }
        }

        private void DoAddPathMapping()
        {
            //TODO: should this also use dependency injection and/or a factory pattern?
            var newPathMappingModel = new PathMappingModel();
            Model.PathMappings.Add(newPathMappingModel);
            PathMappings.Add(new PathMappingViewModel(newPathMappingModel));
        }

        private void DoDeleteSelectedPathMapping()
        {
            if (!CanDeleteSelectedPathMapping() || SelectedPathMapping == null)
            {
                return;
            }

            if (PathMappings.Contains(SelectedPathMapping))
            {
                Model.PathMappings.Remove(SelectedPathMapping.Model);
                PathMappings.Remove(SelectedPathMapping);
            }
        }

        private bool CanDeleteSelectedPathMapping() => SelectedPathMapping != null;
    }
}
