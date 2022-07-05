namespace FrameworkLibrary.Interfaces
{
    public interface IEnvironment
    {
        string LocalApplicationDataPath { get; }
        string ProgramFilesPath { get; }

        string GetApplicationName();

        string GetLocalApplicationFolderPath();

        string GetInstallationFolderPath();

        string GetTempFolderName();
        string GetUserSettingsFileName();
    }
}
