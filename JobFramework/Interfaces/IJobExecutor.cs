namespace JobFramework.Interfaces
{
    public interface IJobExecutor
    {
        Task ExecuteJobAsync(IJob job);
    }
}
