using AutoBackup.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace AutoBackup.ViewModel
{
    public class PathMappingViewModel : ObservableObject
    {
        public PathMappingViewModel(PathMappingModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));

            Source = new SinglePathViewModel(model.Source);
            Target = new SinglePathViewModel(model.Target);
        }

        public PathMappingModel Model { get; }

        public SinglePathViewModel Source { get; }

        public SinglePathViewModel Target { get; }
    }
}
