namespace AutoBackup.Model
{
    public class PathMappingModel
    {
        public SinglePathModel Source { get; set; } = new SinglePathModel();

        public SinglePathModel Target { get; set; } = new SinglePathModel();
    }
}
