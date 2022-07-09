using System.Collections.Generic;
using System.IO;

namespace FrameworkLibrary.Interfaces
{
    public interface IFileSystemUtility
    {
        bool FileExists(string? path);
        bool DirectoryExists(string? path);
        string ReadAllText(string path);
        void WriteAllText(string path, string? contents);
        void DeleteFileIfExists(string? path);
        long GetFileSize(string path);
        void CreateDirectoryChainIfNeeded(string? path);
        IEnumerable<string> GetDirectories(string path);
        IEnumerable<string> GetFiles(string path);
        void CreateDirectory(string? path);
        void DeleteDirectory(string? path);
        Stream GetFileStream(string path, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read);
    }
}
