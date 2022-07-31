namespace AutoBackup.Interfaces
{
    public interface IPathMap
    {
        IPath Source { get; set; }
        IPath Target { get; set; }
    }
}
