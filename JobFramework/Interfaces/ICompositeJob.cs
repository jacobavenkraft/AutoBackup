namespace JobFramework.Interfaces
{
    /// <summary>
    /// A job composed of many smaller child jobs
    /// NOTE: child jobs may also be of type  ICompositeJob
    /// </summary>
    public interface ICompositeJob : IJob
    {
        IEnumerable<IJob> ChildJobs { get; }
    }
}
