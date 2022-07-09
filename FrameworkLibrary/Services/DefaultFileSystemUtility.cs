using FrameworkLibrary.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace FrameworkLibrary.Services
{
    public class DefaultFileSystemUtility : IFileSystemUtility
    {
        public void CreateDirectory(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
        }

        public void CreateDirectoryChainIfNeeded(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var fileInfo = new FileInfo(path);

            if (fileInfo?.Directory == null)
            {
                return;
            }

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }
        }

        public void DeleteDirectory(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public void DeleteFileIfExists(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || !FileExists(path))
            {
                return;
            }

            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                return;
            }

            fileInfo.Delete();
        }

        public bool FileExists(string? path) => File.Exists(path);

        public bool DirectoryExists(string? path) => Directory.Exists(path);

        public IEnumerable<string> GetDirectories(string path) => Directory.GetDirectories(path);

        public IEnumerable<string> GetFiles(string path) => Directory.GetFiles(path);

        public long GetFileSize(string path) => FileExists(path) ? new FileInfo(path).Length : 0;

        public string ReadAllText(string path) => FileExists(path) ? File.ReadAllText(path) : string.Empty;

        public Stream GetFileStream(string path, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read)
        {
            if (!FileExists(path))
            {
                return FileStream.Null;
            }

            return File.Open(path, fileMode, fileAccess, fileShare);
        }

        public void WriteAllText(string path, string? contents) => File.WriteAllText(path, contents);
    }
}
