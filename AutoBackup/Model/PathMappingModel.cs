using AutoBackup.Interfaces;

namespace AutoBackup.Model
{
    public class PathMappingModel : IPathMap
    {
        public IPath Source { get; set; } = new SinglePathModel();

        public IPath Target { get; set; } = new SinglePathModel();
    }
}
