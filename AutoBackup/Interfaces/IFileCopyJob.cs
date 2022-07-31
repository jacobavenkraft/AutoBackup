using JobFramework.Interfaces;

namespace AutoBackup.Interfaces
{
    public interface IFileCopyJob : IPathMapBasedJob, IJob
    {
        long FileSize { get; }
        long ProgressBytes { get; }
        void UpdateByteCount(long lastByteCount);
    }
}
