using JobFramework.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobFramework.Scheduler
{
    /// <summary>
    /// Given an IJob, build the task or subtasks needed to execute that job and run them on the ThreadPool using the provided
    /// TTaskScheduler generic TaskScheduler type.  Each discrete job will be sent to TJobExecutor for execution while each composite
    /// job will be broken down into its discrete child jobs before those discrete jobs are sent to the TJobExecutor.
    /// NOTE: discrete jobs may run in parallel
    /// </summary>
    /// <typeparam name="TJobExecutor">executes the job</typeparam>
    /// <typeparam name="TTaskScheduler">schedules job task on the thread pool</typeparam>
    public class JobScheduler<TJobExecutor, TTaskScheduler> : IJobScheduler
        where TJobExecutor : IJobExecutor
        where TTaskScheduler : TaskScheduler
    {
        private readonly ILogger<JobScheduler<TJobExecutor, TTaskScheduler>> _logger;
        private readonly TJobExecutor _jobExecutor;
        private readonly TTaskScheduler _taskScheduler;

        public JobScheduler(ILogger<JobScheduler<TJobExecutor, TTaskScheduler>> logger, TJobExecutor jobExecutor, TTaskScheduler taskScheduler)
        {
            _logger = logger;
            _jobExecutor = jobExecutor;
            _taskScheduler = taskScheduler;
        }

        public async Task ScheduleJobAsync(IJob job)
        {
            switch (job)
            {
                case ICompositeJob compositeJob:
                    await ScheduleCompositeJobAsync(compositeJob);
                    break;
                default:
                    await ScheduleDiscreteJobAsync(job);
                    break;
            }
        }

        private async Task ScheduleCompositeJobAsync(ICompositeJob compositeJob)
        {
            var jobProgress = compositeJob as IJobProgress;

            if (jobProgress is null)
            {
                _logger.LogInformation($"Composite job [{compositeJob.Description}] does not support progress.");
            }
            
            jobProgress?.InitializeProgress();

            var jobTasks = new List<Task>();

            foreach (var child in compositeJob.ChildJobs)
            {
                jobTasks.Add(ScheduleJobAsync(child));
            }

            var allTasks = Task.WhenAll(jobTasks.ToArray());

            try
            {
                await allTasks;
            }
            catch   //we will handle individual errors below
            {
            }

            jobProgress?.CancellationToken.ThrowIfCancellationRequested();

            int failedTaskCount = jobTasks.Count(t => t.Status == TaskStatus.Faulted);

            if (failedTaskCount > 0)
            {
                throw new Exception($"Composite job [{compositeJob.Description}]: {failedTaskCount} out of {jobTasks.Count} child tasks failed.");
            }
        }

        private async Task ScheduleDiscreteJobAsync(IJob job)
        {
            if (job is ICompositeJob)
            {
                throw new InvalidOperationException($"Invalid call to {nameof(ScheduleDiscreteJobAsync)}.  Param [{nameof(job)}] must NOT be an ICompositeJob.");
            }

            var jobProgress = job as IJobProgress;

            if (jobProgress is null)
            {
                _logger.LogInformation($"Job [{job.Description}] does not support progress.");
            }

            bool jobCompleted = false;

            var maxRetries = Math.Max((jobProgress?.ErrorNumberOfRetries ?? 1), 1);
            var retryDelay = jobProgress != null
                ? TimeSpan.FromSeconds(jobProgress.ErrorRetryDelaySeconds)
                : TimeSpan.Zero;

            int errorCount = 0;

            while (errorCount < maxRetries)
            {
                jobProgress?.CancellationToken.ThrowIfCancellationRequested();  //NOTE: do this before the try-catch because we don't want to retry when the user has cancelled the job

                try
                {
                    jobProgress?.InitializeProgress();

                    var discreteJobTask = Task.Factory.StartNew(async (jobParam) =>
                        {
                            if (jobParam is IJob jobToExecute)
                            {
                                await _jobExecutor.ExecuteJobAsync(jobToExecute);
                            }
                        },
                        job,
                        jobProgress?.CancellationToken ?? CancellationToken.None,
                        TaskCreationOptions.LongRunning,
                        _taskScheduler).Unwrap();

                    await discreteJobTask;

                    jobCompleted = true;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    _logger.LogError(ex, $"Exception executing discrete job [{job.Description}] : {errorCount} attempt(s) failed.");
                    await Task.Delay(retryDelay);
                }
            }

            if (!jobCompleted)
            {
                throw new Exception($"Aborted discrete job [{job.Description}]: failed to complete after {errorCount} attempts.");
            }
        }
    }
}
