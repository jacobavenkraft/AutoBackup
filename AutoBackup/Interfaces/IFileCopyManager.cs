using System.Threading.Tasks;

namespace AutoBackup.Interfaces
{
    public interface IFileCopyManager
    {
        Task EnqueuePathMaps(IPathMap[] maps);
        Task EnqueuePathMap(IPathMap map);
    }
}
