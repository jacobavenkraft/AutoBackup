using AutoBackup.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoBackup.ViewModel
{
    public class PathMappingViewModel : ObservableObject
    {
        public PathMappingViewModel(PathMappingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Source = new SinglePathViewModel(model.Source);
            Target = new SinglePathViewModel(model.Target);
        }

        public SinglePathViewModel Source { get; }

        public SinglePathViewModel Target { get; }
    }
}
