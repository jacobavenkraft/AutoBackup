using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    public class DefaultPersistenceServiceTests
    {
        public class MockUserSettings
        {
            public string? Setting1 { get; set; }
            public string? Setting2 { get; set; }
        }

        [Fact]
        public void DefaultPersistenceService_LoadSettings_ReturnsValidSettings_WhenFileDoesNotExist()
        {
            //setup
            var mockUserSettingsFileName = $"C:\\4D87C952-AFF3-41E7-84AF-259713279ADC\\usersettings.json";
            var mockUtils = new MockUtils<DefaultPersistenceService<MockUserSettings>>();
            var environmentMock = mockUtils.GetMock<IEnvironment>();
            var fileUtilsMock = mockUtils.GetMock<IFileSystemUtility>();

            //testing when file does NOT exist
            fileUtilsMock.FileExists(Arg.Any<string>()).ReturnsForAnyArgs(false);
            environmentMock.GetUserSettingsFileName().ReturnsForAnyArgs(mockUserSettingsFileName);

            var defaultPersistenceService = mockUtils.GetMainInstance();

            //execute
            var loadedSettings = defaultPersistenceService.LoadSettings();

            //assert
            Assert.NotNull(loadedSettings);
        }

        [Fact]
        public void DefaultPersistenceService_LoadSettings_LoadsValidSettingsFromExistingFile()
        {
            //setup
            var mockSetting1Value = "Setting1Value";
            var mockSetting2Value = "Setting2Value";

            var mockSettings = new MockUserSettings() { Setting1 = mockSetting1Value, Setting2 = mockSetting2Value };
            var mockSettingsJson = JsonConvert.SerializeObject(mockSettings);

            var mockUserSettingsFileName = $"C:\\4D87C952-AFF3-41E7-84AF-259713279ADC\\usersettings.json";
            var mockUtils = new MockUtils<DefaultPersistenceService<MockUserSettings>>();
            var environmentMock = mockUtils.GetMock<IEnvironment>();
            var fileUtilsMock = mockUtils.GetMock<IFileSystemUtility>();

            //testing when file does NOT exist
            fileUtilsMock.FileExists(mockUserSettingsFileName).Returns(true);
            fileUtilsMock.ReadAllText(mockUserSettingsFileName).Returns(mockSettingsJson);
            environmentMock.GetUserSettingsFileName().ReturnsForAnyArgs(mockUserSettingsFileName);

            var defaultPersistenceService = mockUtils.GetMainInstance();

            //execute
            var loadedSettings = defaultPersistenceService.LoadSettings();

            //assert
            Assert.NotNull(loadedSettings);
            Assert.Equal(mockSetting1Value, loadedSettings.Setting1);
            Assert.Equal(mockSetting2Value, loadedSettings.Setting2);
        }

        [Fact]
        public void DefaultPersistenceService_LoadSettings_LoadsValidSettings_WhenExistingFileHasInvalidSettings()
        {
            //setup
            var mockSetting1Value = "Setting1Value";
            var mockSetting2Value = "Setting2Value";

            var mockSettings = new MockUserSettings() { Setting1 = mockSetting1Value, Setting2 = mockSetting2Value };
        
            var mockUserSettingsFileName = $"C:\\4D87C952-AFF3-41E7-84AF-259713279ADC\\usersettings.json";
            var mockUtils = new MockUtils<DefaultPersistenceService<MockUserSettings>>();
            var environmentMock = mockUtils.GetMock<IEnvironment>();
            var fileUtilsMock = mockUtils.GetMock<IFileSystemUtility>();

            //testing when file does NOT exist
            fileUtilsMock.FileExists(mockUserSettingsFileName).Returns(true);
            fileUtilsMock.ReadAllText(mockUserSettingsFileName).Returns($"A bunch of garbage file content that isn't even json.");
            environmentMock.GetUserSettingsFileName().ReturnsForAnyArgs(mockUserSettingsFileName);

            var defaultPersistenceService = mockUtils.GetMainInstance();

            //execute
            var loadedSettings = defaultPersistenceService.LoadSettings();

            //assert
            Assert.NotNull(loadedSettings);
            Assert.NotEqual(mockSetting1Value, loadedSettings.Setting1);
            Assert.NotEqual(mockSetting2Value, loadedSettings.Setting2);
        }

        [Fact]
        public void DefaultPersistenceService_LoadSettings_LogsWarning_WhenExistingFileHasInvalidSettings()
        {
            //setup
            var mockSetting1Value = "Setting1Value";
            var mockSetting2Value = "Setting2Value";

            var mockSettings = new MockUserSettings() { Setting1 = mockSetting1Value, Setting2 = mockSetting2Value };

            var mockLogger = new MockLogger<IPersistenceService<MockUserSettings>>();

            var mockUserSettingsFileName = $"C:\\4D87C952-AFF3-41E7-84AF-259713279ADC\\usersettings.json";
            var mockUtils = new MockUtils<DefaultPersistenceService<MockUserSettings>>();
            mockUtils.AddSingleton<ILogger<IPersistenceService<MockUserSettings>>>(mockLogger);
            var environmentMock = mockUtils.GetMock<IEnvironment>();
            var fileUtilsMock = mockUtils.GetMock<IFileSystemUtility>();            

            //testing when file does NOT exist
            fileUtilsMock.FileExists(mockUserSettingsFileName).Returns(true);
            fileUtilsMock.ReadAllText(mockUserSettingsFileName).Returns($"A bunch of garbage file content that isn't even json.");
            environmentMock.GetUserSettingsFileName().ReturnsForAnyArgs(mockUserSettingsFileName);

            var defaultPersistenceService = mockUtils.GetMainInstance();

            //execute
            var loadedSettings = defaultPersistenceService.LoadSettings();

            //assert
            Assert.NotNull(loadedSettings);
            mockLogger.ReceivedLogWarning();
        }

        [Fact]
        public void DefaultPersistenceService_SaveSettings_SavesSettingsToFile()
        {
            //setup
            var mockSetting1Value = "Setting1Value";
            var mockSetting2Value = "Setting2Value";

            var mockSettings = new MockUserSettings() { Setting1 = mockSetting1Value, Setting2 = mockSetting2Value };
            var mockSettingsJson = JsonConvert.SerializeObject(mockSettings, Formatting.Indented);

            var mockUserSettingsFileName = $"C:\\4D87C952-AFF3-41E7-84AF-259713279ADC\\usersettings.json";
            var mockUtils = new MockUtils<DefaultPersistenceService<MockUserSettings>>();
            var environmentMock = mockUtils.GetMock<IEnvironment>();
            var fileUtilsMock = mockUtils.GetMock<IFileSystemUtility>();

            //testing when file does NOT exist
            fileUtilsMock.FileExists(mockUserSettingsFileName).Returns(true);
            environmentMock.GetUserSettingsFileName().ReturnsForAnyArgs(mockUserSettingsFileName);

            var defaultPersistenceService = mockUtils.GetMainInstance();

            //execute
            defaultPersistenceService.SaveSettings(mockSettings);

            //assert
            fileUtilsMock.Received().WriteAllText(mockUserSettingsFileName, mockSettingsJson);
        }
    }
}
