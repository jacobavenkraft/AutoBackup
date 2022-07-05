using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Options;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;

namespace FrameworkLibrary.Services
{
    public class DefaultEnvironment : IEnvironment
    {
        private readonly UserOptions _userOptions;

        public DefaultEnvironment(IOptions<UserOptions> options)
        {
            _userOptions = options.Value;
        }

        public string LocalApplicationDataPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public string ProgramFilesPath => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        public string GetApplicationName() => Assembly.GetEntryAssembly()?.GetName()?.Name ?? "UNKNOWN_APP";

        public string GetInstallationFolderPath() => Assembly.GetEntryAssembly()?.Location ?? Path.Combine(ProgramFilesPath, GetApplicationName());
        
        public string GetLocalApplicationFolderPath() => Path.Combine(LocalApplicationDataPath, GetApplicationName());

        public string GetTempFolderName() => Path.Combine(Path.GetTempPath(), GetApplicationName());

        public string GetUserSettingsFileName() => Path.Combine(GetLocalApplicationFolderPath(), _userOptions.SettingsFileName);
    }
}
