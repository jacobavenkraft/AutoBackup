namespace FrameworkLibrary.Interfaces
{
    public interface IPersistenceService<TUserSettingsModel> where TUserSettingsModel : class, new()
    {
        TUserSettingsModel LoadSettings();
        void SaveSettings(TUserSettingsModel settingsModel);
    }
}
