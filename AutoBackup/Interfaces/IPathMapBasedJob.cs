using JobFramework.Interfaces;

namespace AutoBackup.Interfaces
{
    public interface IPathMapBasedJob : IPathMap, IJobProgress, IDescription, IJob
    {
    }
}
