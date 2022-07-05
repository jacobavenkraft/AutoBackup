using AutoBackup.Model;
using CommonServiceLocator;
using FrameworkLibrary.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;

namespace AutoBackup.ViewModel
{
    public class SinglePathViewModel : ObservableObject
    {
        private IRelayCommand? _selectFolderCommand;
        private ICommonDialogService _commonDialogService;

        public SinglePathModel Model { get; }

        public SinglePathViewModel(SinglePathModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            _commonDialogService = ServiceLocator.Current.GetService<ICommonDialogService>() ?? throw new ArgumentNullException(nameof(_commonDialogService));
        }

        public IRelayCommand SelectFolderCommand => _selectFolderCommand ??= new RelayCommand(DoSelectFolder);

        public string Path
        {
            get => Model.Path;
            set => SetProperty(Model.Path, value, Model, (m, newPath) => m.Path = newPath);
        }

        private void DoSelectFolder()
        {
            var newFolder = _commonDialogService.FolderPickerDialog(Path);

            if (!string.IsNullOrWhiteSpace(newFolder))
            {
                Path = newFolder;
            }
        }
    }
}
