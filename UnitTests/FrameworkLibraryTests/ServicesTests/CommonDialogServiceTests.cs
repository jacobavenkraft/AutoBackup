using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Interop;
using FrameworkLibrary.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.FrameworkLibraryTests.Utils;
using Xunit;

namespace UnitTests.FrameworkLibraryTests.ServicesTests
{
    public class CommonDialogServiceTests
    {
        [Fact]
        public void CommonDialogService_FolderPickerDialog_TitleShowsOnForm_OKButtonShowsOnForm()
        {
            //setup

            var mockUtils = new MockUtils<CommonDialogService>();
            var folderPickerDialogMock = mockUtils.GetMock<IFolderPickerDialog>();
            var windowFactoryMock = mockUtils.GetMock<IWindowFactory>();

            windowFactoryMock.CreateWindow<IFolderPickerDialog>().ReturnsForAnyArgs(folderPickerDialogMock);

            var commonDialogService = mockUtils.GetMainInstance();
            //execute
            string expectedTitle = "Mock Title";
            string expectedOkButton = "Mock OK Button";

            commonDialogService.FolderPickerDialog(title: expectedTitle, okButtonText: expectedOkButton);

            //assert
            Assert.Equal(expectedTitle, folderPickerDialogMock.Title);
            Assert.Equal(expectedOkButton, folderPickerDialogMock.OkButtonLabel);
        }

        [Fact]
        public void CommonDialogService_FolderPickerDialog_NullTitle_AssumesFallbackValue()
        {
            //setup

            var mockUtils = new MockUtils<CommonDialogService>();
            var folderPickerDialogMock = mockUtils.GetMock<IFolderPickerDialog>();
            var windowFactoryMock = mockUtils.GetMock<IWindowFactory>();

            windowFactoryMock.CreateWindow<IFolderPickerDialog>().ReturnsForAnyArgs(folderPickerDialogMock);

            var commonDialogService = mockUtils.GetMainInstance();
            //execute
            string expectedTitle = CommonDialogService.FallbackTitle;

            commonDialogService.FolderPickerDialog(title: null);

            //assert
            Assert.Equal(expectedTitle, folderPickerDialogMock.Title);
        }

        [Fact]
        public void CommonDialogService_FolderPickerDialog_NullOkButtonText_AssumesFallbackValue()
        {
            //setup

            var mockUtils = new MockUtils<CommonDialogService>();
            var folderPickerDialogMock = mockUtils.GetMock<IFolderPickerDialog>();
            var windowFactoryMock = mockUtils.GetMock<IWindowFactory>();

            windowFactoryMock.CreateWindow<IFolderPickerDialog>().ReturnsForAnyArgs(folderPickerDialogMock);

            var commonDialogService = mockUtils.GetMainInstance();
            //execute
            string expectedOKButtonText = CommonDialogService.FallbackOkButtonText;

            commonDialogService.FolderPickerDialog(okButtonText: null);

            //assert
            Assert.Equal(expectedOKButtonText, folderPickerDialogMock.OkButtonLabel);
        }

        [Fact]
        public void CommonDialogService_FolderPickerDialog_UseStartingDirectory_WhenStartingDirectoryExists()
        {
            //setup
            var mockDirectoryThatDoesExist = $"C:\\DirectoryThatDoesExist";
            var mockDirectoryThatDoesNotExist = $"C:\\DirectoryThatDoesNotExist";
            var mockUtils = new MockUtils<CommonDialogService>();
            var folderPickerDialogMock = mockUtils.GetMock<IFolderPickerDialog>();
            var windowFactoryMock = mockUtils.GetMock<IWindowFactory>();
            var fileUtilsMock = mockUtils.GetMock<IFileSystemUtility>();

            fileUtilsMock.DirectoryExists(mockDirectoryThatDoesExist).Returns(true);
            fileUtilsMock.DirectoryExists(mockDirectoryThatDoesNotExist).Returns(false);
            windowFactoryMock.CreateWindow<IFolderPickerDialog>().ReturnsForAnyArgs(folderPickerDialogMock);

            var commonDialogService = mockUtils.GetMainInstance();

            //execute
            var expectedStartingDirectory = mockDirectoryThatDoesExist;
            commonDialogService.FolderPickerDialog(startingDirectory: mockDirectoryThatDoesExist);

            //assert
            Assert.Equal(expectedStartingDirectory, folderPickerDialogMock.InputPath);
        }

        [Fact]
        public void CommonDialogService_FolderPickerDialog_IgnoreStartingDirectory_WhenStartingDirectoryDoesNotExist()
        {
            //setup
            var mockDirectoryThatDoesExist = $"C:\\DirectoryThatDoesExist";
            var mockDirectoryThatDoesNotExist = $"C:\\DirectoryThatDoesNotExist";
            var mockUtils = new MockUtils<CommonDialogService>();
            var folderPickerDialogMock = mockUtils.GetMock<IFolderPickerDialog>();
            var windowFactoryMock = mockUtils.GetMock<IWindowFactory>();
            var fileUtilsMock = mockUtils.GetMock<IFileSystemUtility>();

            fileUtilsMock.DirectoryExists(mockDirectoryThatDoesExist).Returns(true);
            fileUtilsMock.DirectoryExists(mockDirectoryThatDoesNotExist).Returns(false);
            windowFactoryMock.CreateWindow<IFolderPickerDialog>().ReturnsForAnyArgs(folderPickerDialogMock);

            var commonDialogService = mockUtils.GetMainInstance();

            //execute
            var expectedStartingDirectory = "";
            commonDialogService.FolderPickerDialog(startingDirectory: mockDirectoryThatDoesNotExist);

            //assert
            Assert.Equal(expectedStartingDirectory, folderPickerDialogMock.InputPath);
        }

        [Fact]
        public void CommonDialogService_FolderPickerDialog_WhenResultIsTrue_ReturnsSelectedPath()
        {
            //setup
            var mockUserSelectedDirectory = $"C:\\DirectoryThatUserChose";

            var mockUtils = new MockUtils<CommonDialogService>();
            var folderPickerDialogMock = mockUtils.GetMock<IFolderPickerDialog>();
            var windowFactoryMock = mockUtils.GetMock<IWindowFactory>();

            windowFactoryMock.CreateWindow<IFolderPickerDialog>().ReturnsForAnyArgs(folderPickerDialogMock);
            folderPickerDialogMock.ResultPath.ReturnsForAnyArgs(mockUserSelectedDirectory);
            folderPickerDialogMock.ShowDialog().Returns(true);

            var commonDialogService = mockUtils.GetMainInstance();

            //execute
            var expectedResult = mockUserSelectedDirectory;
            var actualResult = commonDialogService.FolderPickerDialog();

            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void CommonDialogService_FolderPickerDialog_WhenResultIsFalse_ReturnsNullOrWhitespace()
        {
            //setup
            var mockUserSelectedDirectory = $"C:\\DirectoryThatUserChose";

            var mockUtils = new MockUtils<CommonDialogService>();
            var folderPickerDialogMock = mockUtils.GetMock<IFolderPickerDialog>();
            var windowFactoryMock = mockUtils.GetMock<IWindowFactory>();

            windowFactoryMock.CreateWindow<IFolderPickerDialog>().ReturnsForAnyArgs(folderPickerDialogMock);
            folderPickerDialogMock.ResultPath.ReturnsForAnyArgs(mockUserSelectedDirectory);
            folderPickerDialogMock.ShowDialog().Returns(false);

            var commonDialogService = mockUtils.GetMainInstance();

            //execute
            var actualResult = commonDialogService.FolderPickerDialog();

            //assert
            Assert.True(string.IsNullOrWhiteSpace(actualResult));
        }


    }
}
