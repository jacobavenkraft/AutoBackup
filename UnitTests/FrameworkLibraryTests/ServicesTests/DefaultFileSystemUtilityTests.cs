using FrameworkLibrary.Services;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace UnitTests.FrameworkLibraryTests.ServicesTests
{
    public class DefaultFileSystemUtilityTests
    {
        [Fact]
        public void DefaultFileSystemUtility_CreateDirectory_DoesCreateDirectory()
        {
            //setup
            var testDirectoryName = $"C:\\5FF93205-9DAA-47ED-87A6-916EE51FD1C6";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var directoryExistsBeforeCreateDirectoryCall = Directory.Exists(testDirectoryName);
            defaultFileSystemUtility.CreateDirectory(testDirectoryName);
            var directoryExistsAfterCreateDirectoryCall = Directory.Exists(testDirectoryName);

            //assert
            Assert.False(directoryExistsBeforeCreateDirectoryCall);
            Assert.True(directoryExistsAfterCreateDirectoryCall);

            //teardown
            Directory.Delete(testDirectoryName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void DefaultFileSystemUtility_CreateDirectory_WithNullOrWhitespaceDoesNotRaiseException(string? emptyDirectory)
        {
            //setup
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            bool exceptionWasRaised = false;
            try
            {
#pragma warning disable CS8604 // Suppress warning because we are testing whether null argument will raise an exception.
                defaultFileSystemUtility.CreateDirectory(emptyDirectory);
#pragma warning restore CS8604
            }
            catch
            {
                exceptionWasRaised = true;
            }
            
            //assert
            Assert.False(exceptionWasRaised);
        }

        [Fact]
        public void DefaultFileSystemUtility_CreateDirectory_BadDirectoryNameRaisesException()
        {
            //setup
            var mockDirectoryName = $"C:\\<BADDIR>";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            bool exceptionWasRaised = false;
            try
            {
                defaultFileSystemUtility.CreateDirectory(mockDirectoryName);
            }
            catch
            {
                exceptionWasRaised = true;
            }

            //assert
            Assert.True(exceptionWasRaised);
        }

        [Fact]
        public void DefaultFileSystemUtility_CreateDirectoryChainIfNeeded_DoesCreateDirectoryChain()
        {
            //setup
            var rootDirectoryName = $"C:\\AF96BCAC-2E79-4D0D-87FD-0BC9C1321FAA";
            var fullDirectoryName = $"C:\\AF96BCAC-2E79-4D0D-87FD-0BC9C1321FAA\\411A2BBF-891B-471F-B860-EC6F39FB24CB";
            var fileName = $"C:\\AF96BCAC-2E79-4D0D-87FD-0BC9C1321FAA\\411A2BBF-891B-471F-B860-EC6F39FB24CB\\mockFile.txt";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //pre-cleanup in case there is some weird issue where the root directory already existed
            if (Directory.Exists(fullDirectoryName))
            {
                Directory.Delete(fullDirectoryName, true);
            }

            //execute
            var directoryExistsBeforeTest = Directory.Exists(fullDirectoryName);
            defaultFileSystemUtility.CreateDirectoryChainIfNeeded(fileName);
            var directoryExistsAfterTest = Directory.Exists(fullDirectoryName);

            //assert
            Assert.False(directoryExistsBeforeTest);
            Assert.True(directoryExistsAfterTest);

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }

        [Fact]
        public void DefaultFileSystemUtility_CreateDirectoryChainIfNeeded_FileWithNullDirectoryDoesNotRaiseException()
        {
            //setup
            var fileNameWithoutDirectory = $"\\\\server.domain.mock\\mockFile.txt";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            var fileInfo = new FileInfo(fileNameWithoutDirectory);

            //execute

            var directoryIsNull = fileInfo.Directory == null;

            var exceptionWasRaised = false;
            try
            {
                defaultFileSystemUtility.CreateDirectoryChainIfNeeded(fileNameWithoutDirectory);
            }
            catch
            {
                exceptionWasRaised = true;
            }
            
            //assert
            Assert.False(exceptionWasRaised);
            Assert.True(directoryIsNull);
        }

        [Fact]
        public void DefaultFileSystemUtility_DeleteDirectory_DoesDeleteDirectory()
        {
            //setup
            var rootDirectoryName = $"C:\\2C9066EF-2BA7-4405-BDC1-7F8D344B2419";
            var fullDirectoryName = $"C:\\2C9066EF-2BA7-4405-BDC1-7F8D344B2419\\712F8903-6C7A-499A-BA3A-B09ED4C04BF0";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //pre-cleanup in case there is some weird issue where the root directory already existed
            if (!Directory.Exists(fullDirectoryName))
            {
                Directory.CreateDirectory(fullDirectoryName);
            }

            //execute
            var directoryExistsBeforeTest = Directory.Exists(fullDirectoryName);
            defaultFileSystemUtility.DeleteDirectory(fullDirectoryName);
            var directoryExistsAfterTest = Directory.Exists(fullDirectoryName);

            //assert
            Assert.True(directoryExistsBeforeTest);
            Assert.False(directoryExistsAfterTest);

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void DefaultFileSystemUtility_DeleteDirectory_WithNullOrWhitespaceDoesNotRaiseException(string? emptyDirectory)
        {
            //setup
            var defaultFileSystemUtility = new DefaultFileSystemUtility();
            
            //execute
            bool exceptionWasRaised = false;
            try
            {
#pragma warning disable CS8604 // Suppress warning because we want to test what happens if it is null
                defaultFileSystemUtility.DeleteDirectory(emptyDirectory);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            catch
            {
                exceptionWasRaised = true;
            }

            //assert
            Assert.False(exceptionWasRaised);
        }

        [Fact]
        public void DefaultFileSystemUtility_DeleteFileIfExists_WhenFileExistsDoesDeleteFile()
        {
            //setup
            var rootDirectoryName = $"C:\\7DB90B8C-6345-4E52-B27B-89688D62A6A4";
            var fileName = $"C:\\7DB90B8C-6345-4E52-B27B-89688D62A6A4\\8B850A28-4E34-455E-93E0-2BFB93D0F235\\mock.txt";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //make sure our mock file exists
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var fileInfo = new FileInfo(fileName);
            fileInfo?.Directory?.Create();

            File.WriteAllText(fileName, "Mock text file content");

            //execute
            var fileExistsBeforeTest = File.Exists(fileName);
            defaultFileSystemUtility.DeleteFileIfExists(fileName);
            var fileExistsAfterTest = Directory.Exists(fileName);

            //assert
            Assert.True(fileExistsBeforeTest);
            Assert.False(fileExistsAfterTest);

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }

        [Fact]
        public void DefaultFileSystemUtility_DeleteFileIfExists_WhenFileDoesNotExistsDoesNotRaiseException()
        {
            //setup
            var fileName = $"C:\\B764806C-9B69-44A6-929A-711DAF42EC8A\\CF61C535-0C7A-4EA0-BB53-DB469D3ABB36\\mock.txt";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //make sure our mock file is NOT there
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            //execute
            var fileExistsBeforeTest = true;
            var fileExistsAfterTest = true;
            var exceptionWasRaised = false;
            try
            {
                fileExistsBeforeTest = File.Exists(fileName);
                defaultFileSystemUtility.DeleteFileIfExists(fileName);
                fileExistsAfterTest = Directory.Exists(fileName);
            }
            catch
            {
                exceptionWasRaised = true;
            }

            //assert
            Assert.False(fileExistsBeforeTest);
            Assert.False(fileExistsAfterTest);
            Assert.False(exceptionWasRaised);
        }

        [Fact]
        public void DefaultFileSystemUtility_FileExists_WorksAsExpected()
        {
            //setup
            var doesNotExistFileName = $"C:\\E08A2CA7-B649-4712-A3B8-41C690DB4676\\invalidMock.txt";
            var existsSubDir = $"C:\\9E99B92E-42B0-4B0A-AC55-4895B396D646";
            var existsFileName = $"C:\\9E99B92E-42B0-4B0A-AC55-4895B396D646\\mock.txt";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            if (File.Exists(doesNotExistFileName))
            {
                File.Delete(doesNotExistFileName);
            }

            if (File.Exists(existsFileName))
            {
                File.Delete(existsFileName);
            }

            if (!Directory.Exists(existsSubDir))
            {
                Directory.CreateDirectory(existsSubDir);
            }

            File.WriteAllText(existsFileName, "Mock file content");

            //execute
            var existsFileNameDoesExist = defaultFileSystemUtility.FileExists(existsFileName);
            var doesNotExistFileNameDoesNotExist = !defaultFileSystemUtility.FileExists(doesNotExistFileName);

            //assert
            Assert.True(existsFileNameDoesExist);
            Assert.True(doesNotExistFileNameDoesNotExist);

            //teardown
            File.Delete(existsFileName);
            Directory.Delete(existsSubDir, true);
        }

        [Fact]
        public void DefaultFileSystemUtility_DirectoryExists_WorksAsExpected()
        {
            //setup
            var doesNotExistDirectoryName = $"C:\\6F4071FA-4BCF-4A3E-8E45-C8C6C3CC3E62";
            var existsDirectoryName = $"C:\\F113D1CA-007D-4C97-983D-7CDED567F236";
            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            if (Directory.Exists(doesNotExistDirectoryName))
            {
                Directory.Delete(doesNotExistDirectoryName, true);
            }

            if (!Directory.Exists(existsDirectoryName))
            {
                Directory.CreateDirectory(existsDirectoryName);
            }

            //execute
            var existsDirectoryNameDoesExist = defaultFileSystemUtility.DirectoryExists(existsDirectoryName);
            var doesNotExistDirectoryNameDoesNotExist = !defaultFileSystemUtility.DirectoryExists(doesNotExistDirectoryName);

            //assert
            Assert.True(existsDirectoryNameDoesExist);
            Assert.True(doesNotExistDirectoryNameDoesNotExist);

            //teardown
            Directory.Delete(existsDirectoryName, true);
        }

        [Fact]
        public void DefaultFileSystemUtility_GetDirectories_WorksAsExpected()
        {
            //setup
            var rootDirectoryName = $"C:\\94DD529B-8B3F-4197-9798-58D48FBA4CF8";

            var expectedDirectories = new string[]
            {
                $"C:\\94DD529B-8B3F-4197-9798-58D48FBA4CF8\\544343E2-F792-40E6-A22D-8567A37CAB33",
                $"C:\\94DD529B-8B3F-4197-9798-58D48FBA4CF8\\95C06BD7-DCEC-4DBB-822C-1F38BF37AC3C",
                $"C:\\94DD529B-8B3F-4197-9798-58D48FBA4CF8\\2849DB36-4B2F-43C3-8F42-7F7D98CC1E4C"
            };

            ForceDirectoriesToExist(expectedDirectories);

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var actualDirectories = defaultFileSystemUtility.GetDirectories(rootDirectoryName);

            //assert
            foreach (var expectedDirectory in expectedDirectories)
            {
                Assert.Contains(expectedDirectory, actualDirectories);
            }

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }


        private void ForceDirectoriesToExist(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        [Fact]
        public void DefaultFileSystemUtility_GetFiles_WorksAsExpected()
        {
            //setup
            var rootDirectoryName = $"C:\\58B617A7-FF68-4234-BB80-63608BFDD114";

            var expectedFiles = new string[]
            {
                $"C:\\58B617A7-FF68-4234-BB80-63608BFDD114\\mockFile1.txt",
                $"C:\\58B617A7-FF68-4234-BB80-63608BFDD114\\mockFile2.txt",
                $"C:\\58B617A7-FF68-4234-BB80-63608BFDD114\\mockFile3.txt"
            };

            ForceFilesToExist(expectedFiles);

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var actualFiles = defaultFileSystemUtility.GetFiles(rootDirectoryName);

            //assert
            foreach (var expectedFile in expectedFiles)
            {
                Assert.Contains(expectedFile, actualFiles);
            }

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }

        private void ForceFileToExist(string fileName, string content = "Mock file content")
        {
            var fileInfo = new FileInfo(fileName);

            fileInfo?.Directory?.Create();

            File.WriteAllText(fileName, content);
        }

        private void ForceFilesToExist(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                ForceFileToExist(file);
            }
        }

        [Fact]
        public void DefaultFileSystemUtility_GetFileSize_WhenFileExists_ReturnsActualFileSize()
        {
            //setup
            var rootDirectoryName = $"C:\\3B8E2240-FA3D-48BD-BB80-0C6E7AFAE138";
            var fileName = $"C:\\3B8E2240-FA3D-48BD-BB80-0C6E7AFAE138\\mock.txt";

            if (Directory.Exists(rootDirectoryName))
            {
                Directory.Delete(rootDirectoryName, true);
            }

            ForceFileToExist(fileName);

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var fileInfo = new FileInfo(fileName);

            var expectedSize = fileInfo.Length;

            var actualSize = defaultFileSystemUtility.GetFileSize(fileName);

            //assert
            Assert.Equal(expectedSize, actualSize);

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }

        [Fact]
        public void DefaultFileSystemUtility_GetFileSize_WhenFileDoesNotExist_ReturnsZero()
        {
            //setup
            var rootDirectoryName = $"C:\\DF37CD8F-9894-4633-A384-EBA99264D218";
            var fileName = $"C:\\DF37CD8F-9894-4633-A384-EBA99264D218\\mock.txt";

            if (Directory.Exists(rootDirectoryName))
            {
                Directory.Delete(rootDirectoryName, true);
            }

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var expectedSize = 0;

            var actualSize = defaultFileSystemUtility.GetFileSize(fileName);

            //assert
            Assert.Equal(expectedSize, actualSize);
        }

        [Fact]
        public void DefaultFileSystemUtility_ReadAllText_WhenFileExists_ReturnsActualFileContent()
        {
            //setup
            var rootDirectoryName = $"C:\\2815066A-F461-43FD-9085-C87B128A97B7";
            var fileName = $"C:\\2815066A-F461-43FD-9085-C87B128A97B7\\mock.txt";

            if (Directory.Exists(rootDirectoryName))
            {
                Directory.Delete(rootDirectoryName, true);
            }

            ForceFileToExist(fileName);

            var expectedContent = File.ReadAllText(fileName);

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var actualContent = defaultFileSystemUtility.ReadAllText(fileName);

            //assert
            Assert.Equal(expectedContent, actualContent);

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }

        [Fact]
        public void DefaultFileSystemUtility_ReadAllText_WhenFileDoesNotExist_ReturnsEmptyString()
        {
            //setup
            var rootDirectoryName = $"C:\\34FBE6F5-F93B-4D0E-B12F-5190FD97E85B";
            var fileName = $"C:\\34FBE6F5-F93B-4D0E-B12F-5190FD97E85B\\mock.txt";

            if (Directory.Exists(rootDirectoryName))
            {
                Directory.Delete(rootDirectoryName, true);
            }

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var actualContent = defaultFileSystemUtility.ReadAllText(fileName);

            //assert
            Assert.True(string.IsNullOrWhiteSpace(actualContent));
        }

        [Fact]
        public void DefaultFileSystemUtility_WriteAllText_WorksAsExpected()
        {
            //setup
            var rootDirectoryName = $"C:\\89C356D0-F526-4396-BEC8-B112A2B912F0";
            var fileName = $"C:\\89C356D0-F526-4396-BEC8-B112A2B912F0\\mock.txt";

            if (Directory.Exists(rootDirectoryName))
            {
                Directory.Delete(rootDirectoryName, true);
            }

            Directory.CreateDirectory(rootDirectoryName);

            var expectedContent = "This is some mock content";

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            defaultFileSystemUtility.WriteAllText(fileName, expectedContent);

            var actualContent = File.ReadAllText(fileName);

            //assert
            Assert.Equal(expectedContent, actualContent);

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }

        [Fact]
        public void DefaultFileSystemUtility_GetFileStream_WhenFileDoesNotExist_ReturnsFileStreamNull()
        {
            //setup
            var rootDirectoryName = $"C:\\200E7626-130B-4297-8B2F-472F2FA8EACB";
            var fileName = $"C:\\200E7626-130B-4297-8B2F-472F2FA8EACB\\mock.txt";

            if (Directory.Exists(rootDirectoryName))
            {
                Directory.Delete(rootDirectoryName, true);
            }

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var actualFileStream = defaultFileSystemUtility.GetFileStream(fileName);

            //assert
            Assert.Equal(FileStream.Null, actualFileStream);
        }

        [Fact]
        public void DefaultFileSystemUtility_GetFileStream_WhenFileDoesExist_ReturnsValidStream()
        {
            //setup
            var rootDirectoryName = $"C:\\C579F266-320E-4B1E-B2B8-6378424779DC";
            var fileName = $"C:\\C579F266-320E-4B1E-B2B8-6378424779DC\\mock.txt";

            if (Directory.Exists(rootDirectoryName))
            {
                Directory.Delete(rootDirectoryName, true);
            }

            ForceFileToExist(fileName);

            var expectedContent = File.ReadAllText(fileName);

            var defaultFileSystemUtility = new DefaultFileSystemUtility();

            //execute
            var actualContent = string.Empty;

            using (var actualFileStream = defaultFileSystemUtility.GetFileStream(fileName))
            {
                using (var reader = new StreamReader(actualFileStream))
                {
                    actualContent = reader.ReadToEnd();
                }
            }

            //assert
            Assert.Equal(expectedContent, actualContent);

            //teardown
            Directory.Delete(rootDirectoryName, true);
        }
    }
}
