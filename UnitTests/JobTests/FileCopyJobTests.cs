using AutoBackup.Job;
using FrameworkLibrary.Interfaces;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.JobTests
{
    public class FileCopyJobTests
    {
        [Fact]
        public void FileCopyJob_UpdateByteCount_FiresProgressChangedEvent()
        {
            //setup
            long mockFileSize = 50000;
            
            var fileUtilsMock = NSubstitute.Substitute.For<IFileSystemUtility>();

            fileUtilsMock.GetFileSize(Arg.Any<string>()).Returns(mockFileSize);

            var fileCopyJob = new FileCopyJob(fileUtilsMock, "c:\\source\\any.txt", "c:\\target\\any.txt");
            AbstractPathMapJob.Precision = 2;

            int progressChangeCount = 0;

            fileCopyJob.ProgressChanged += (sender, oldValue, newValue) => progressChangeCount++;

            //execute
            while (fileCopyJob.ProgressBytes != fileCopyJob.FileSize)
            {
                fileCopyJob.UpdateByteCount(1);
            }

            //assert
            Assert.Equal(100, progressChangeCount);
        }

        [Fact]
        public void FileCopyJob_UpdateByteCount_IncrementsProgress()
        {
            //setup
            long mockFileSize = 50000;

            var fileUtilsMock = NSubstitute.Substitute.For<IFileSystemUtility>();

            fileUtilsMock.GetFileSize(Arg.Any<string>()).Returns(mockFileSize);

            var fileCopyJob = new FileCopyJob(fileUtilsMock, "c:\\source\\any.txt", "c:\\target\\any.txt");
            AbstractPathMapJob.Precision = 2;

            List<int> percentList = new List<int>();

            fileCopyJob.ProgressChanged += (sender, oldValue, newValue) => percentList.Add(sender.ProgressPercent);

            //execute
            while(fileCopyJob.ProgressBytes != fileCopyJob.FileSize)
            {
                fileCopyJob.UpdateByteCount(1);
            }
            
            //assert
            Assert.Equal(100, percentList.Count);
            for (int i = 1; i <= 100; i++)
            {
                Assert.Equal(i, percentList[i - 1]);
            }
        }

        [Fact]
        public void FileCopyJob_UpdateByteCount_CannotExceedFileSize()
        {
            //setup
            long mockFileSize = 50000;

            var fileUtilsMock = NSubstitute.Substitute.For<IFileSystemUtility>();

            fileUtilsMock.GetFileSize(Arg.Any<string>()).Returns(mockFileSize);

            var fileCopyJob = new FileCopyJob(fileUtilsMock, "c:\\source\\any.txt", "c:\\target\\any.txt");
            AbstractPathMapJob.Precision = 2;

            List<int> percentList = new List<int>();

            fileCopyJob.ProgressChanged += (sender, oldValue, newValue) => percentList.Add(sender.ProgressPercent);

            //execute
            while (fileCopyJob.ProgressBytes != fileCopyJob.FileSize)
            {
                fileCopyJob.UpdateByteCount(1);
            }

            fileCopyJob.UpdateByteCount(1); //this shouldn't do anything
            fileCopyJob.UpdateByteCount(1);
            fileCopyJob.UpdateByteCount(1);

            //assert
            Assert.Equal(fileCopyJob.ProgressBytes, fileCopyJob.FileSize);
            Assert.Equal(100, percentList.Count);
            for (int i = 1; i <= 100; i++)
            {
                Assert.Equal(i, percentList[i - 1]);
            }
        }

        [Fact]
        public void FileCopyJob_UpdateByteCount_CannotGoNegative()
        {
            //setup
            long mockFileSize = 50000;

            var fileUtilsMock = NSubstitute.Substitute.For<IFileSystemUtility>();

            fileUtilsMock.GetFileSize(Arg.Any<string>()).Returns(mockFileSize);

            var fileCopyJob = new FileCopyJob(fileUtilsMock, "c:\\source\\any.txt", "c:\\target\\any.txt");
            
            //execute
            fileCopyJob.UpdateByteCount(-mockFileSize);

            //assert
            Assert.Equal(0, fileCopyJob.ProgressBytes);
            Assert.NotEqual(fileCopyJob.ProgressBytes, fileCopyJob.FileSize);
        }

        [Fact]
        public void FileCopyJob_UpdateByteCount_CannotGoBackwards()
        {
            //setup
            long mockFileSize = 50000;

            var fileUtilsMock = NSubstitute.Substitute.For<IFileSystemUtility>();

            fileUtilsMock.GetFileSize(Arg.Any<string>()).Returns(mockFileSize);

            var fileCopyJob = new FileCopyJob(fileUtilsMock, "c:\\source\\any.txt", "c:\\target\\any.txt");

            //execute
            fileCopyJob.UpdateByteCount(25000);
            fileCopyJob.UpdateByteCount(-mockFileSize);

            Assert.Equal(25000, fileCopyJob.ProgressBytes);

            fileCopyJob.UpdateByteCount(mockFileSize);
            fileCopyJob.UpdateByteCount(-mockFileSize);

            //assert
            Assert.Equal(fileCopyJob.ProgressBytes, fileCopyJob.FileSize);
        }
    }
}
