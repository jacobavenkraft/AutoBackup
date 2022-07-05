using AutoBackup.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace AutoBackup.ViewModel
{
    public class SinglePathViewModel : ObservableObject
    {
        public SinglePathModel Model { get; }

        public SinglePathViewModel(SinglePathModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public string Path
        {
            get => Model.Path;
            set => SetProperty(Model.Path, value, Model, (m, newPath) => m.Path = newPath);
        }
    }
}
