using System.Collections.Generic;

namespace AutoBackup.Model
{
    public class ApplicationSettingsModel
    {
        public List<PathMappingModel> PathMappings { get; set; } = new List<PathMappingModel>();
    }
}
