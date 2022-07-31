using AutoBackup.Interfaces;
using FrameworkLibrary.Interfaces;
using System;

namespace AutoBackup.Job
{
    public class FileCopyJob : AbstractPathMapJob, IFileCopyJob
    {
        private IFileSystemUtility _fileSystemUtility;

        public FileCopyJob(IFileSystemUtility fileSystemUtility, string sourceFilePath, string targetFilePath)
        {
            _fileSystemUtility = fileSystemUtility ?? throw new ArgumentNullException(nameof(fileSystemUtility));
            Source.Path = sourceFilePath;
            Target.Path = targetFilePath;
            FileSize = _fileSystemUtility.GetFileSize(sourceFilePath);
        }

        public long FileSize { get; }

        public long ProgressBytes { get; private set; } = 0;

        public void UpdateByteCount(long lastByteCount)
        {
            lastByteCount = Math.Max(0, lastByteCount); //can't be less than 0

            var newProgressBytes = ProgressBytes + lastByteCount;

            newProgressBytes = Math.Min(FileSize, newProgressBytes);    //can't be greater than file size
            
            ProgressBytes = newProgressBytes;

            UpdateProgress((double)ProgressBytes / (double)FileSize);
        }
    }
}
