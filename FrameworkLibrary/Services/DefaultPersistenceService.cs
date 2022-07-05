using FrameworkLibrary.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FrameworkLibrary.Services
{
    public class DefaultPersistenceService<TUserSettingsModel> : IPersistenceService<TUserSettingsModel> where TUserSettingsModel : class, new()
    {
        private readonly IFileSystemUtility _fileSystem;
        private readonly IEnvironment _environment;
        private readonly ILogger<DefaultPersistenceService<TUserSettingsModel>> _logger;

        static DefaultPersistenceService()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
            };
        }

        public DefaultPersistenceService(ILogger<DefaultPersistenceService<TUserSettingsModel>> logger, IFileSystemUtility fileSystem, IEnvironment environment)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _environment = environment;
        }

        public TUserSettingsModel LoadSettings()
        {
            var fileName = GetFileName();

            if (!_fileSystem.FileExists(fileName))
            {
                _logger.LogWarning($"Settings file does not exist: [{fileName}].  Creating empty settings file.");
                return new TUserSettingsModel();
            }

            var settingsJson = _fileSystem.ReadAllText(fileName);
            var settingsModel = JsonConvert.DeserializeObject<TUserSettingsModel>(settingsJson);

            if (settingsModel == null)
            {
                _logger.LogWarning($"Invalid content in settings file: [{fileName}].  Creating empty settings file.");
                return new TUserSettingsModel();
            }

            return settingsModel;
        }

        public void SaveSettings(TUserSettingsModel settingsModel)
        {
            var settingsJson = JsonConvert.SerializeObject(settingsModel, Formatting.Indented);
            var fileName = GetFileName();

            _fileSystem.CreateDirectoryChainIfNeeded(fileName);
            _fileSystem.WriteAllText(fileName, settingsJson);
        }

        private string GetFileName() => _environment.GetUserSettingsFileName();
    }
}
