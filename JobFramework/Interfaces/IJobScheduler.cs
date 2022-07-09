namespace JobFramework.Interfaces
{
    public interface IJobScheduler
    {
        Task ScheduleJobAsync(IJob job);
    }
}
