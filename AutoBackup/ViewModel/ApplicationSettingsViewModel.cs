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
            set => SetProperty(ref _selectedPathMapping, value);
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
            PathMappings.Add(new PathMappingViewModel(new PathMappingModel()));
        }

        private void DoDeleteSelectedPathMapping()
        {
            if (!CanDeleteSelectedPathMapping() || SelectedPathMapping == null)
            {
                return;
            }

            if (PathMappings.Contains(SelectedPathMapping))
            {
                PathMappings.Remove(SelectedPathMapping);
            }
        }

        private bool CanDeleteSelectedPathMapping() => SelectedPathMapping != null;
    }
}
