using AutoBackup.Interfaces;

namespace AutoBackup.Model
{
    public class SinglePathModel : IPath
    {
        public string Path { get; set; } = string.Empty;
    }
}
